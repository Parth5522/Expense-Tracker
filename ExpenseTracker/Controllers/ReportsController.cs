using ExpenseTracker.Services;
using ExpenseTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers;

[Authorize]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
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

    public async Task<IActionResult> Index(DateTime? from, DateTime? to, int? year)
    {
        var userId = GetUserId();

        // If a year is selected, show the full year overview
        if (year.HasValue)
        {
            from = new DateTime(year.Value, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            to   = new DateTime(year.Value, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        }
        else
        {
            from ??= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            to   ??= DateTime.UtcNow;
        }

        var filter = new ExpenseFilterViewModel { UserId = userId, FromDate = from, ToDate = to, PageSize = int.MaxValue };
        var expenseResult = await _expenseService.GetFilteredExpensesAsync(filter);
        var allIncomes = await _incomeService.GetAllIncomesAsync(userId);
        var filteredIncomes = allIncomes.Where(i => i.Date >= from && i.Date <= to).ToList();

        // Available years for the year selector (from data or last 3 years)
        var allExpenses = await _expenseService.GetFilteredExpensesAsync(
            new ExpenseFilterViewModel { UserId = userId, PageSize = int.MaxValue });
        var expenseYears = allExpenses.Expenses.Select(e => e.Date.Year);
        var incomeYears  = allIncomes.Select(i => i.Date.Year);
        var availableYears = expenseYears.Concat(incomeYears)
            .Append(DateTime.UtcNow.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToList();

        // Monthly breakdown for the selected period
        var monthlyBreakdowns = new List<MonthlyBreakdown>();
        if (year.HasValue || (to.Value - from.Value).TotalDays > 31)
        {
            // Group by year-month across the range
            var expsByMonth = expenseResult.Expenses
                .GroupBy(e => new { e.Date.Year, e.Date.Month })
                .ToDictionary(g => g.Key, g => g.Sum(e => e.AmountInBaseCurrency));
            var incByMonth = filteredIncomes
                .GroupBy(i => new { i.Date.Year, i.Date.Month })
                .ToDictionary(g => g.Key, g => g.Sum(i => i.AmountInBaseCurrency));

            var cursor = new DateTime(from.Value.Year, from.Value.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var end    = new DateTime(to.Value.Year, to.Value.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            while (cursor <= end)
            {
                var key = new { cursor.Year, cursor.Month };
                monthlyBreakdowns.Add(new MonthlyBreakdown
                {
                    Month     = cursor.Month,
                    MonthName = cursor.ToString("MMM yyyy"),
                    Expenses  = expsByMonth.TryGetValue(key, out var e) ? e : 0,
                    Income    = incByMonth.TryGetValue(key, out var i) ? i : 0
                });
                cursor = cursor.AddMonths(1);
            }
        }

        var model = new ReportViewModel
        {
            FromDate           = from.Value,
            ToDate             = to.Value,
            SelectedYear       = year,
            AvailableYears     = availableYears,
            TotalExpenses      = expenseResult.Expenses.Sum(e => e.AmountInBaseCurrency),
            TotalIncome        = filteredIncomes.Sum(i => i.AmountInBaseCurrency),
            ExpensesByCategory = expenseResult.Expenses.GroupBy(e => e.Category)
                                    .ToDictionary(g => g.Key.ToString(), g => g.Sum(e => e.AmountInBaseCurrency)),
            IncomeBySource     = filteredIncomes.GroupBy(i => i.Source)
                                    .ToDictionary(g => g.Key.ToString(), g => g.Sum(i => i.AmountInBaseCurrency)),
            MonthlyBreakdowns  = monthlyBreakdowns,
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
