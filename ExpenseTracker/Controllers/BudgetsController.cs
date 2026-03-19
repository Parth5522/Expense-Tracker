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
    private readonly INotificationService _notificationService;

    public BudgetsController(IBudgetService budgetService, IExpenseService expenseService, INotificationService notificationService)
    {
        _budgetService = budgetService;
        _expenseService = expenseService;
        _notificationService = notificationService;
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

        // Build spending recommendations based on last 3 months vs current budgets
        var recommendations = new List<string>();
        foreach (var b in budgets)
        {
            decimal totalSpent = 0;
            int monthsCounted = 0;
            for (int i = 1; i <= 3; i++)
            {
                var pastDate = DateTime.UtcNow.AddMonths(-i);
                var pastSpent = await _expenseService.GetSpentAmountAsync(GetUserId(), pastDate.Month, pastDate.Year, b.Category);
                if (pastSpent > 0) { totalSpent += pastSpent; monthsCounted++; }
            }
            if (monthsCounted > 0)
            {
                var avg = totalSpent / monthsCounted;
                var categoryLabel = b.Category.HasValue ? b.Category.ToString() : "Overall";
                if (avg > b.Amount * 1.1m)
                    recommendations.Add($"Your average {categoryLabel} spending ({avg:N2}) exceeds your budget ({b.Amount:N2}). Consider raising the budget or reducing spending.");
                else if (avg < b.Amount * 0.6m)
                    recommendations.Add($"You consistently spend less than your {categoryLabel} budget (avg {avg:N2} vs {b.Amount:N2}). You could lower the budget by ~{(b.Amount - avg):N2}.");
            }
        }
        ViewBag.Recommendations = recommendations;

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
            var categoryLabel = budget.Category.HasValue ? budget.Category.ToString() : "Overall";
            await _notificationService.CreateNotificationAsync(GetUserId(), NotificationType.General,
                $"Budget created: {categoryLabel} — {budget.Currency} {budget.Amount:N2} for {new DateTime(budget.Year, budget.Month, 1):MMMM yyyy}.");
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
            var categoryLabel = budget.Category.HasValue ? budget.Category.ToString() : "Overall";
            await _notificationService.CreateNotificationAsync(GetUserId(), NotificationType.General,
                $"Budget updated: {categoryLabel} — {budget.Currency} {budget.Amount:N2} for {new DateTime(budget.Year, budget.Month, 1):MMMM yyyy}.");
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
        var categoryLabel = budget.Category.HasValue ? budget.Category.ToString() : "Overall";
        await _budgetService.DeleteBudgetAsync(id);
        await _notificationService.CreateNotificationAsync(GetUserId(), NotificationType.General,
            $"Budget deleted: {categoryLabel} for {new DateTime(budget.Year, budget.Month, 1):MMMM yyyy}.");
        TempData["Success"] = "Budget deleted.";
        return RedirectToAction(nameof(Index));
    }
}
