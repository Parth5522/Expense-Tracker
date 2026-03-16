using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;

namespace ExpenseTracker.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userCount = await _userManager.Users.CountAsync();
        var expenseCount = await _context.Expenses.CountAsync();
        var incomeCount = await _context.Incomes.CountAsync();
        ViewBag.UserCount = userCount;
        ViewBag.ExpenseCount = expenseCount;
        ViewBag.IncomeCount = incomeCount;
        return View();
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
}
