using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers;

[Authorize(Roles = "Admin")]
public class RecurringController : Controller
{
    private readonly IRecurringTransactionService _recurringService;

    public RecurringController(IRecurringTransactionService recurringService)
    {
        _recurringService = recurringService;
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    public async Task<IActionResult> Index()
    {
        var items = await _recurringService.GetRecurringTransactionsAsync(GetUserId());
        return View(items);
    }

    public IActionResult Create() => View(new RecurringTransaction { StartDate = DateTime.UtcNow, NextRunAt = DateTime.UtcNow });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RecurringTransaction model)
    {
        if (ModelState.IsValid)
        {
            model.UserId = GetUserId();
            model.StartDate = DateTime.SpecifyKind(model.StartDate, DateTimeKind.Utc);
            model.NextRunAt = DateTime.SpecifyKind(model.NextRunAt, DateTimeKind.Utc);
            if (model.EndDate.HasValue)
                model.EndDate = DateTime.SpecifyKind(model.EndDate.Value, DateTimeKind.Utc);
            await _recurringService.CreateAsync(model);
            TempData["Success"] = "Recurring transaction created.";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _recurringService.GetByIdAsync(id);
        if (item == null || item.UserId != GetUserId()) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RecurringTransaction model)
    {
        if (id != model.Id) return NotFound();
        var existing = await _recurringService.GetByIdAsync(id);
        if (existing == null || existing.UserId != GetUserId()) return NotFound();
        if (ModelState.IsValid)
        {
            model.UserId = GetUserId();
            model.StartDate = DateTime.SpecifyKind(model.StartDate, DateTimeKind.Utc);
            model.NextRunAt = DateTime.SpecifyKind(model.NextRunAt, DateTimeKind.Utc);
            if (model.EndDate.HasValue)
                model.EndDate = DateTime.SpecifyKind(model.EndDate.Value, DateTimeKind.Utc);
            await _recurringService.UpdateAsync(model);
            TempData["Success"] = "Recurring transaction updated.";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _recurringService.GetByIdAsync(id);
        if (item == null || item.UserId != GetUserId()) return NotFound();
        await _recurringService.DeleteAsync(id);
        TempData["Success"] = "Recurring transaction deleted.";
        return RedirectToAction(nameof(Index));
    }
}
