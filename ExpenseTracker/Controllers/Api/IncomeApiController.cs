using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers.Api;

[ApiController]
[Route("api/v1/income")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class IncomeApiController : ControllerBase
{
    private readonly IIncomeService _incomeService;

    public IncomeApiController(IIncomeService incomeService) => _incomeService = incomeService;

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var incomes = await _incomeService.GetAllIncomesAsync(GetUserId());
        return Ok(incomes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var income = await _incomeService.GetIncomeByIdAsync(id);
        if (income == null || income.UserId != GetUserId()) return NotFound();
        return Ok(income);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Income income)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        income.UserId = GetUserId();
        income.Date = DateTime.SpecifyKind(income.Date, DateTimeKind.Utc);
        income.AmountInBaseCurrency = income.Amount * income.ExchangeRate;
        var created = await _incomeService.CreateIncomeAsync(income);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Income income)
    {
        if (id != income.Id) return BadRequest();
        var existing = await _incomeService.GetIncomeByIdAsync(id);
        if (existing == null || existing.UserId != GetUserId()) return NotFound();
        income.UserId = GetUserId();
        income.UpdatedAt = DateTime.UtcNow;
        income.Date = DateTime.SpecifyKind(income.Date, DateTimeKind.Utc);
        income.AmountInBaseCurrency = income.Amount * income.ExchangeRate;
        await _incomeService.UpdateIncomeAsync(income);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _incomeService.GetIncomeByIdAsync(id);
        if (existing == null || existing.UserId != GetUserId()) return NotFound();
        await _incomeService.DeleteIncomeAsync(id);
        return NoContent();
    }
}
