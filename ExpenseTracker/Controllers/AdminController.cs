using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;

namespace ExpenseTracker.Controllers;

[Authorize(Roles = "Admin")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly ICurrencyService _currencyService;

    public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, ICurrencyService currencyService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _currencyService = currencyService;
    }

    public async Task<IActionResult> Index()
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1);

        var users = await _userManager.Users.ToListAsync();

        var monthlyExpenses = await _context.Expenses
            .Where(e => e.Date >= monthStart && e.Date < monthEnd)
            .GroupBy(e => e.UserId)
            .Select(g => new { UserId = g.Key, Total = g.Sum(e => e.Amount) })
            .ToListAsync();

        var monthlyIncomes = await _context.Incomes
            .Where(i => i.Date >= monthStart && i.Date < monthEnd)
            .GroupBy(i => i.UserId)
            .Select(g => new { UserId = g.Key, Total = g.Sum(i => i.Amount) })
            .ToListAsync();

        var expenseDict = monthlyExpenses
            .Where(x => x.UserId != null)
            .ToDictionary(x => x.UserId!, x => x.Total);
        var incomeDict = monthlyIncomes
            .Where(x => x.UserId != null)
            .ToDictionary(x => x.UserId!, x => x.Total);

        var userSummaries = users.Select(u => new
        {
            User = u,
            MonthlyExpense = expenseDict.TryGetValue(u.Id, out var exp) ? exp : 0m,
            MonthlyIncome = incomeDict.TryGetValue(u.Id, out var inc) ? inc : 0m
        }).ToList();

        ViewBag.UserCount = users.Count;
        ViewBag.MonthLabel = now.ToString("MMMM yyyy");
        ViewBag.UserSummaries = userSummaries;

        ViewBag.TotalExpenses = await _context.Expenses.SumAsync(e => (decimal?)e.Amount) ?? 0m;
        ViewBag.TotalIncome = await _context.Incomes.SumAsync(i => (decimal?)i.Amount) ?? 0m;
        ViewBag.TotalExpenseCount = await _context.Expenses.CountAsync();
        ViewBag.TotalIncomeCount = await _context.Incomes.CountAsync();

        return View();
    }

    public async Task<IActionResult> AllExpenses(int page = 1, int pageSize = 20)
    {
        var userMap = await _userManager.Users
            .ToDictionaryAsync(u => u.Id, u => u.DisplayName ?? u.UserName ?? u.Email ?? u.Id);

        var query = _context.Expenses.OrderByDescending(e => e.Date);
        var totalItems = await query.CountAsync();
        var expenses = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.UserMap = userMap;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        return View(expenses);
    }

    public async Task<IActionResult> AllIncome(int page = 1, int pageSize = 20)
    {
        var userMap = await _userManager.Users
            .ToDictionaryAsync(u => u.Id, u => u.DisplayName ?? u.UserName ?? u.Email ?? u.Id);

        var query = _context.Incomes.OrderByDescending(i => i.Date);
        var totalItems = await query.CountAsync();
        var incomes = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.UserMap = userMap;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        return View(incomes);
    }

    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.ToListAsync();
        var usersWithRoles = new List<(ApplicationUser User, IList<string> Roles)>();
        foreach (var u in users)
            usersWithRoles.Add((u, await _userManager.GetRolesAsync(u)));
        ViewBag.UsersWithRoles = usersWithRoles;
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAdmin(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();
        if (await _userManager.IsInRoleAsync(user, "Admin"))
            await _userManager.RemoveFromRoleAsync(user, "Admin");
        else
            await _userManager.AddToRoleAsync(user, "Admin");
        TempData["Success"] = "User role updated.";
        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();
        await _userManager.DeleteAsync(user);
        TempData["Success"] = "User deleted.";
        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> Currencies()
    {
        var currencies = await _context.Currencies.OrderBy(c => c.Code).ToListAsync();
        return View(currencies);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRate(string code, decimal rateToUsd)
    {
        if (rateToUsd <= 0)
        {
            TempData["Error"] = "Exchange rate must be greater than zero.";
            return RedirectToAction(nameof(Currencies));
        }
        var updated = await _currencyService.UpdateRateAsync(code, rateToUsd);
        TempData[updated ? "Success" : "Error"] = updated
            ? $"{code} exchange rate updated successfully."
            : $"Currency '{code}' not found.";
        return RedirectToAction(nameof(Currencies));
    }
}
