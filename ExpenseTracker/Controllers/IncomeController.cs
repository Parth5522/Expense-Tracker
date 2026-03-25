using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers;

[Authorize]
public class IncomeController : Controller
{
    private readonly IIncomeService _incomeService;
    private readonly ITagService _tagService;
    private readonly ICurrencyService _currencyService;

    public IncomeController(IIncomeService incomeService, ITagService tagService, ICurrencyService currencyService)
    {
        _incomeService = incomeService;
        _tagService = tagService;
        _currencyService = currencyService;
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    public async Task<IActionResult> Index()
    {
        var incomes = await _incomeService.GetAllIncomesAsync(GetUserId());
        ViewBag.Currencies = await _currencyService.GetAllCurrenciesAsync();
        return View(incomes);
    }

    public async Task<IActionResult> Details(int id)
    {
        var income = await _incomeService.GetIncomeByIdAsync(id);
        if (income == null || income.UserId != GetUserId()) return NotFound();
        return View(income);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Tags = await _tagService.GetTagsAsync(GetUserId());
        ViewBag.Currencies = await _currencyService.GetAllCurrenciesAsync();
        return View(new Income { Date = DateTime.Now });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Income income)
    {
        if (ModelState.IsValid)
        {
            income.UserId = GetUserId();
            income.Date = DateTime.SpecifyKind(income.Date, DateTimeKind.Utc);
            income.AmountInBaseCurrency = income.Amount * income.ExchangeRate;
            await _incomeService.CreateIncomeAsync(income);
            TempData["Success"] = "Income added successfully.";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Tags = await _tagService.GetTagsAsync(GetUserId());
        ViewBag.Currencies = await _currencyService.GetAllCurrenciesAsync();
        return View(income);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var income = await _incomeService.GetIncomeByIdAsync(id);
        if (income == null || income.UserId != GetUserId()) return NotFound();
        ViewBag.Currencies = await _currencyService.GetAllCurrenciesAsync();
        return View(income);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Income income)
    {
        if (id != income.Id) return NotFound();
        var existing = await _incomeService.GetIncomeByIdAsync(id);
        if (existing == null || existing.UserId != GetUserId()) return NotFound();
        if (ModelState.IsValid)
        {
            existing.Title = income.Title;
            existing.Amount = income.Amount;
            existing.Source = income.Source;
            existing.Date = DateTime.SpecifyKind(income.Date, DateTimeKind.Utc);
            existing.Currency = income.Currency;
            existing.ExchangeRate = income.ExchangeRate;
            existing.Description = income.Description;
            existing.AmountInBaseCurrency = existing.Amount * existing.ExchangeRate;
            existing.UpdatedAt = DateTime.UtcNow;
            await _incomeService.UpdateIncomeAsync(existing);
            TempData["Success"] = "Income updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Currencies = await _currencyService.GetAllCurrenciesAsync();
        return View(income);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var income = await _incomeService.GetIncomeByIdAsync(id);
        if (income == null || income.UserId != GetUserId()) return NotFound();
        return View(income);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var income = await _incomeService.GetIncomeByIdAsync(id);
        if (income == null || income.UserId != GetUserId()) return NotFound();
        await _incomeService.DeleteIncomeAsync(id);
        TempData["Success"] = "Income deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Export(DateTime? from, DateTime? to)
    {
        var userId = GetUserId();
        var incomes = await _incomeService.GetAllIncomesAsync(userId);
        if (from.HasValue) incomes = incomes.Where(i => i.Date >= from.Value).ToList();
        if (to.HasValue) incomes = incomes.Where(i => i.Date <= to.Value).ToList();
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Id,Title,Amount,Currency,Source,Date,Description");
        foreach (var i in incomes)
            sb.AppendLine($"{i.Id},\"{i.Title.Replace("\"","\"\"")}\",{i.Amount},{i.Currency},{i.Source},{i.Date:yyyy-MM-dd},\"{(i.Description ?? "").Replace("\"","\"\"")}\"");
        return File(System.Text.Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "income.csv");
    }
}
