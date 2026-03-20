using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers.Api;

[ApiController]
[Route("api/v1/goals")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class GoalsApiController : ControllerBase
{
    private readonly IGoalService _goalService;

    public GoalsApiController(IGoalService goalService) => _goalService = goalService;

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var goals = await _goalService.GetGoalsAsync(GetUserId());
        return Ok(goals);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var goal = await _goalService.GetGoalByIdAsync(id);
        if (goal == null || goal.UserId != GetUserId()) return NotFound();
        return Ok(goal);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Goal goal)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        goal.UserId = GetUserId();
        if (goal.TargetDate.HasValue)
            goal.TargetDate = DateTime.SpecifyKind(goal.TargetDate.Value, DateTimeKind.Utc);
        var created = await _goalService.CreateGoalAsync(goal);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Goal goal)
    {
        if (id != goal.Id) return BadRequest();
        var existing = await _goalService.GetGoalByIdAsync(id);
        if (existing == null || existing.UserId != GetUserId()) return NotFound();
        goal.UserId = GetUserId();
        goal.CreatedAt = existing.CreatedAt;
        goal.UpdatedAt = DateTime.UtcNow;
        if (goal.TargetDate.HasValue)
            goal.TargetDate = DateTime.SpecifyKind(goal.TargetDate.Value, DateTimeKind.Utc);
        await _goalService.UpdateGoalAsync(goal);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _goalService.GetGoalByIdAsync(id);
        if (existing == null || existing.UserId != GetUserId()) return NotFound();
        await _goalService.DeleteGoalAsync(id);
        return NoContent();
    }
}
