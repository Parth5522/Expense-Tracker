using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers;

[Authorize(Roles = "Admin")]
public class GoalsController : Controller
{
    private readonly IGoalService _goalService;
    private readonly INotificationService _notificationService;

    public GoalsController(IGoalService goalService, INotificationService notificationService)
    {
        _goalService = goalService;
        _notificationService = notificationService;
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    public async Task<IActionResult> Index()
    {
        var goals = await _goalService.GetGoalsAsync(GetUserId());
        return View(goals);
    }

    public IActionResult Create() => View(new Goal { TargetDate = DateTime.UtcNow.AddMonths(6) });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Goal goal)
    {
        if (ModelState.IsValid)
        {
            goal.UserId = GetUserId();
            if (goal.TargetDate.HasValue)
                goal.TargetDate = DateTime.SpecifyKind(goal.TargetDate.Value, DateTimeKind.Utc);
            await _goalService.CreateGoalAsync(goal);
            TempData["Success"] = "Goal created.";
            return RedirectToAction(nameof(Index));
        }
        return View(goal);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var goal = await _goalService.GetGoalByIdAsync(id);
        if (goal == null || goal.UserId != GetUserId()) return NotFound();
        return View(goal);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Goal goal)
    {
        if (id != goal.Id) return NotFound();
        var existing = await _goalService.GetGoalByIdAsync(id);
        if (existing == null || existing.UserId != GetUserId()) return NotFound();
        if (ModelState.IsValid)
        {
            goal.UserId = GetUserId();
            goal.UpdatedAt = DateTime.UtcNow;
            if (goal.TargetDate.HasValue)
                goal.TargetDate = DateTime.SpecifyKind(goal.TargetDate.Value, DateTimeKind.Utc);
            var wasAchieved = existing.IsAchieved;
            await _goalService.UpdateGoalAsync(goal);
            if (!wasAchieved && goal.CurrentAmount >= goal.TargetAmount)
                await _notificationService.CreateNotificationAsync(GetUserId(), NotificationType.GoalAchieved, $"Congratulations! You achieved your goal: {goal.Name}");
            TempData["Success"] = "Goal updated.";
            return RedirectToAction(nameof(Index));
        }
        return View(goal);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var goal = await _goalService.GetGoalByIdAsync(id);
        if (goal == null || goal.UserId != GetUserId()) return NotFound();
        await _goalService.DeleteGoalAsync(id);
        TempData["Success"] = "Goal deleted.";
        return RedirectToAction(nameof(Index));
    }
}
