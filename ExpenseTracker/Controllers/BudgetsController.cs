using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers;

[Authorize]
public class BudgetsController : Controller
{
    private readonly IBudgetService _budgetService;
    private readonly IExpenseService _expenseService;

    public BudgetsController(IBudgetService budgetService, IExpenseService expenseService)
    {
        _budgetService = budgetService;
        _expenseService = expenseService;
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    public async Task<IActionResult> Index(int? month, int? year)
    {
        var m = month ?? DateTime.UtcNow.Month;
        var y = year ?? DateTime.UtcNow.Year;
        var budgets = await _budgetService.GetBudgetsAsync(GetUserId(), m, y);
        var budgetStatuses = new List<(Budget Budget, decimal Spent, bool Exceeded)>();
        foreach (var b in budgets)
        {
            var spent = await _expenseService.GetSpentAmountAsync(GetUserId(), m, y, b.Category);
            budgetStatuses.Add((b, spent, spent > b.Amount));
        }
        ViewBag.Month = m;
        ViewBag.Year = y;
        ViewBag.BudgetStatuses = budgetStatuses;
        return View(budgets);
    }

    public IActionResult Create() => View(new Budget { Month = DateTime.UtcNow.Month, Year = DateTime.UtcNow.Year });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Budget budget)
    {
        if (ModelState.IsValid)
        {
            budget.UserId = GetUserId();
            await _budgetService.CreateBudgetAsync(budget);
            TempData["Success"] = "Budget created.";
            return RedirectToAction(nameof(Index));
        }
        return View(budget);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var budget = await _budgetService.GetBudgetByIdAsync(id);
        if (budget == null || budget.UserId != GetUserId()) return NotFound();
        return View(budget);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Budget budget)
    {
        if (id != budget.Id) return NotFound();
        var existing = await _budgetService.GetBudgetByIdAsync(id);
        if (existing == null || existing.UserId != GetUserId()) return NotFound();
        if (ModelState.IsValid)
        {
            budget.UserId = GetUserId();
            await _budgetService.UpdateBudgetAsync(budget);
            TempData["Success"] = "Budget updated.";
            return RedirectToAction(nameof(Index));
        }
        return View(budget);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var budget = await _budgetService.GetBudgetByIdAsync(id);
        if (budget == null || budget.UserId != GetUserId()) return NotFound();
        await _budgetService.DeleteBudgetAsync(id);
        TempData["Success"] = "Budget deleted.";
        return RedirectToAction(nameof(Index));
    }
}
