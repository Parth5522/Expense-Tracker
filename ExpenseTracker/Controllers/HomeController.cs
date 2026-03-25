using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers;

public class HomeController : Controller
{
    private readonly IExpenseService _expenseService;
    private readonly IIncomeService _incomeService;
    private readonly IGoalService _goalService;
    private readonly IBudgetService _budgetService;
    private readonly INotificationService _notificationService;

    public HomeController(IExpenseService expenseService, IIncomeService incomeService, IGoalService goalService, IBudgetService budgetService, INotificationService notificationService)
    {
        _expenseService = expenseService;
        _incomeService = incomeService;
        _goalService = goalService;
        _budgetService = budgetService;
        _notificationService = notificationService;
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    [Authorize]
    public async Task<IActionResult> Index()
    {
        if (User.IsInRole("Admin"))
            return RedirectToAction("Index", "Admin");

        var userId = GetUserId();
        var dashboard = await _expenseService.GetDashboardDataAsync(userId);
        dashboard.TotalIncome = await _incomeService.GetTotalIncomeAsync(userId);
        dashboard.Goals = await _goalService.GetGoalsAsync(userId);
        dashboard.Budgets = await _budgetService.GetBudgetsAsync(userId, DateTime.UtcNow.Month, DateTime.UtcNow.Year);
        dashboard.UnreadNotifications = await _notificationService.GetUnreadCountAsync(userId);
        return View(dashboard);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ExpenseTracker.Models.ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
