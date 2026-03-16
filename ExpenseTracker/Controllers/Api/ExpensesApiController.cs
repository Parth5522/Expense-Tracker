using ExpenseTracker.Models;
using ExpenseTracker.Services;
using ExpenseTracker.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers.Api;

[ApiController]
[Route("api/v1/expenses")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ExpensesApiController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesApiController(IExpenseService expenseService) => _expenseService = expenseService;

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string? category, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var filter = new ExpenseFilterViewModel
        {
            UserId = GetUserId(),
            FromDate = from,
            ToDate = to,
            Category = category != null && Enum.TryParse<ExpenseCategory>(category, out var cat) ? cat : null,
            PageNumber = page,
            PageSize = pageSize
        };
        var result = await _expenseService.GetFilteredExpensesAsync(filter);
        return Ok(new { result.Expenses, result.TotalItems, result.PageNumber, result.PageSize });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var expense = await _expenseService.GetExpenseByIdAsync(id);
        if (expense == null || expense.UserId != GetUserId()) return NotFound();
        return Ok(expense);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Expense expense)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        expense.UserId = GetUserId();
        expense.Date = DateTime.SpecifyKind(expense.Date, DateTimeKind.Utc);
        expense.AmountInBaseCurrency = expense.Amount * expense.ExchangeRate;
        var created = await _expenseService.CreateExpenseAsync(expense);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Expense expense)
    {
        if (id != expense.Id) return BadRequest();
        var existing = await _expenseService.GetExpenseByIdAsync(id);
        if (existing == null || existing.UserId != GetUserId()) return NotFound();
        if (!ModelState.IsValid) return BadRequest(ModelState);
        expense.UserId = GetUserId();
        expense.UpdatedAt = DateTime.UtcNow;
        expense.Date = DateTime.SpecifyKind(expense.Date, DateTimeKind.Utc);
        expense.AmountInBaseCurrency = expense.Amount * expense.ExchangeRate;
        await _expenseService.UpdateExpenseAsync(expense);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _expenseService.GetExpenseByIdAsync(id);
        if (existing == null || existing.UserId != GetUserId()) return NotFound();
        await _expenseService.DeleteExpenseAsync(id);
        return NoContent();
    }
}
