using ExpenseTracker.Services;
using ExpenseTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly IExpenseService _expenseService;
    private readonly IIncomeService _incomeService;
    private readonly IReportService _reportService;

    public ReportsController(IExpenseService expenseService, IIncomeService incomeService, IReportService reportService)
    {
        _expenseService = expenseService;
        _incomeService = incomeService;
        _reportService = reportService;
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    public async Task<IActionResult> Index(DateTime? from, DateTime? to)
    {
        var userId = GetUserId();
        var fromDate = from ?? DateTime.UtcNow.AddMonths(-1);
        var toDate = to ?? DateTime.UtcNow;

        var filter = new ExpenseFilterViewModel { UserId = userId, FromDate = fromDate, ToDate = toDate, PageSize = int.MaxValue };
        var expenseResult = await _expenseService.GetFilteredExpensesAsync(filter);
        var allIncomes = await _incomeService.GetAllIncomesAsync(userId);
        var filteredIncomes = allIncomes.Where(i => i.Date >= fromDate && i.Date <= toDate).ToList();

        var model = new ReportViewModel
        {
            FromDate = fromDate,
            ToDate = toDate,
            TotalExpenses = expenseResult.Expenses.Sum(e => e.AmountInBaseCurrency),
            TotalIncome = filteredIncomes.Sum(i => i.AmountInBaseCurrency),
            ExpensesByCategory = expenseResult.Expenses.GroupBy(e => e.Category).ToDictionary(g => g.Key.ToString(), g => g.Sum(e => e.AmountInBaseCurrency)),
            IncomeBySource = filteredIncomes.GroupBy(i => i.Source).ToDictionary(g => g.Key.ToString(), g => g.Sum(i => i.AmountInBaseCurrency)),
        };

        return View(model);
    }

    public async Task<IActionResult> ExportExpenses(DateTime? from, DateTime? to)
    {
        var bytes = await _reportService.ExportExpensesToCsvAsync(GetUserId(), from, to);
        return File(bytes, "text/csv", "expenses-export.csv");
    }

    public async Task<IActionResult> ExportIncome(DateTime? from, DateTime? to)
    {
        var bytes = await _reportService.ExportIncomesToCsvAsync(GetUserId(), from, to);
        return File(bytes, "text/csv", "income-export.csv");
    }
}
