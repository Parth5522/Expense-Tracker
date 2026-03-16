using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers.Api;

[ApiController]
[Route("api/v1/budgets")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class BudgetsApiController : ControllerBase
{
    private readonly IBudgetService _budgetService;

    public BudgetsApiController(IBudgetService budgetService) => _budgetService = budgetService;

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? month, [FromQuery] int? year)
    {
        var m = month ?? DateTime.UtcNow.Month;
        var y = year ?? DateTime.UtcNow.Year;
        var budgets = await _budgetService.GetBudgetsAsync(GetUserId(), m, y);
        return Ok(budgets);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var budget = await _budgetService.GetBudgetByIdAsync(id);
        if (budget == null || budget.UserId != GetUserId()) return NotFound();
        return Ok(budget);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Budget budget)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        budget.UserId = GetUserId();
        var created = await _budgetService.CreateBudgetAsync(budget);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Budget budget)
    {
        if (id != budget.Id) return BadRequest();
        var existing = await _budgetService.GetBudgetByIdAsync(id);
        if (existing == null || existing.UserId != GetUserId()) return NotFound();
        budget.UserId = GetUserId();
        await _budgetService.UpdateBudgetAsync(budget);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _budgetService.GetBudgetByIdAsync(id);
        if (existing == null || existing.UserId != GetUserId()) return NotFound();
        await _budgetService.DeleteBudgetAsync(id);
        return NoContent();
    }
}
