using ExpenseTracker.Models;
using ExpenseTracker.Services;
using ExpenseTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers;

[Authorize]
public class ExpensesController : Controller
{
    private readonly IExpenseService _expenseService;
    private readonly ITagService _tagService;
    private readonly IAttachmentService _attachmentService;
    private readonly ICurrencyService _currencyService;

    public ExpensesController(IExpenseService expenseService, ITagService tagService, IAttachmentService attachmentService, ICurrencyService currencyService)
    {
        _expenseService = expenseService;
        _tagService = tagService;
        _attachmentService = attachmentService;
        _currencyService = currencyService;
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    public async Task<IActionResult> Index(ExpenseFilterViewModel filter)
    {
        filter.UserId = GetUserId();
        var result = await _expenseService.GetFilteredExpensesAsync(filter);
        ViewBag.Tags = await _tagService.GetTagsAsync(GetUserId());
        ViewBag.Currencies = await _currencyService.GetAllCurrenciesAsync();
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var expense = await _expenseService.GetExpenseByIdAsync(id);
        if (expense == null || expense.UserId != GetUserId()) return NotFound();
        return View(expense);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Tags = await _tagService.GetTagsAsync(GetUserId());
        ViewBag.Currencies = await _currencyService.GetAllCurrenciesAsync();
        return View(new Expense { Date = DateTime.Now });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Expense expense, int[]? selectedTags, IFormFile? attachment)
    {
        if (ModelState.IsValid)
        {
            expense.UserId = GetUserId();
            expense.Date = DateTime.SpecifyKind(expense.Date, DateTimeKind.Utc);
            expense.AmountInBaseCurrency = expense.Amount * expense.ExchangeRate;
            var created = await _expenseService.CreateExpenseAsync(expense);

            if (selectedTags?.Length > 0)
                await _expenseService.SetTagsAsync(created.Id, selectedTags);

            if (attachment != null && attachment.Length > 0)
                await _attachmentService.UploadAsync(attachment, created.Id, GetUserId());

            TempData["Success"] = "Expense created successfully.";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Tags = await _tagService.GetTagsAsync(GetUserId());
        ViewBag.Currencies = await _currencyService.GetAllCurrenciesAsync();
        return View(expense);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var expense = await _expenseService.GetExpenseByIdAsync(id);
        if (expense == null || expense.UserId != GetUserId()) return NotFound();
        ViewBag.Tags = await _tagService.GetTagsAsync(GetUserId());
        ViewBag.Currencies = await _currencyService.GetAllCurrenciesAsync();
        ViewBag.SelectedTags = expense.ExpenseTags.Select(et => et.TagId).ToList();
        return View(expense);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Expense expense, int[]? selectedTags)
    {
        if (id != expense.Id) return NotFound();
        var existing = await _expenseService.GetExpenseByIdAsync(id);
        if (existing == null || existing.UserId != GetUserId()) return NotFound();

        if (ModelState.IsValid)
        {
            expense.UserId = GetUserId();
            expense.Date = DateTime.SpecifyKind(expense.Date, DateTimeKind.Utc);
            expense.AmountInBaseCurrency = expense.Amount * expense.ExchangeRate;
            expense.UpdatedAt = DateTime.UtcNow;
            await _expenseService.UpdateExpenseAsync(expense);

            if (selectedTags != null)
                await _expenseService.SetTagsAsync(id, selectedTags);

            TempData["Success"] = "Expense updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Tags = await _tagService.GetTagsAsync(GetUserId());
        ViewBag.Currencies = await _currencyService.GetAllCurrenciesAsync();
        return View(expense);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var expense = await _expenseService.GetExpenseByIdAsync(id);
        if (expense == null || expense.UserId != GetUserId()) return NotFound();
        return View(expense);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var expense = await _expenseService.GetExpenseByIdAsync(id);
        if (expense == null || expense.UserId != GetUserId()) return NotFound();
        await _expenseService.DeleteExpenseAsync(id);
        TempData["Success"] = "Expense deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Export(DateTime? from, DateTime? to)
    {
        var userId = GetUserId();
        var filter = new ExpenseFilterViewModel { UserId = userId, FromDate = from, ToDate = to, PageSize = int.MaxValue };
        var result = await _expenseService.GetFilteredExpensesAsync(filter);
        var csv = GenerateCsv(result.Expenses);
        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "expenses.csv");
    }

    private static string GenerateCsv(IEnumerable<Expense> expenses)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Id,Title,Amount,Currency,Category,Date,Description");
        foreach (var e in expenses)
            sb.AppendLine($"{e.Id},{Escape(e.Title)},{e.Amount},{e.Currency},{e.Category},{e.Date:yyyy-MM-dd},{Escape(e.Description ?? "")}");
        return sb.ToString();
    }

    private static string Escape(string val) => $"\"{val.Replace("\"", "\"\"")}\"";
}
