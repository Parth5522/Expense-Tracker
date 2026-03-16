using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public class IncomeService : IIncomeService
{
    private readonly ApplicationDbContext _context;

    public IncomeService(ApplicationDbContext context) => _context = context;

    public async Task<List<Income>> GetAllIncomesAsync(string userId) =>
        await _context.Incomes.Where(i => i.UserId == userId).OrderByDescending(i => i.Date).ToListAsync();

    public async Task<Income?> GetIncomeByIdAsync(int id) =>
        await _context.Incomes.FindAsync(id);

    public async Task<Income> CreateIncomeAsync(Income income)
    {
        income.CreatedAt = DateTime.UtcNow;
        _context.Incomes.Add(income);
        await _context.SaveChangesAsync();
        return income;
    }

    public async Task<Income> UpdateIncomeAsync(Income income)
    {
        income.UpdatedAt = DateTime.UtcNow;
        _context.Entry(income).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return income;
    }

    public async Task<bool> DeleteIncomeAsync(int id)
    {
        var income = await _context.Incomes.FindAsync(id);
        if (income == null) return false;
        _context.Incomes.Remove(income);
        await _context.SaveChangesAsync();
        return true;
    }
}
