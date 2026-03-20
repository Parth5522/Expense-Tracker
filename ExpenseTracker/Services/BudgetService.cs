using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public class BudgetService : IBudgetService
{
    private readonly ApplicationDbContext _context;

    public BudgetService(ApplicationDbContext context) => _context = context;

    public async Task<List<Budget>> GetBudgetsAsync(string userId, int month, int year) =>
        await _context.Budgets.Where(b => b.UserId == userId && b.Month == month && b.Year == year).ToListAsync();

    public async Task<Budget?> GetBudgetByIdAsync(int id) =>
        await _context.Budgets.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);

    public async Task<Budget> CreateBudgetAsync(Budget budget)
    {
        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();
        return budget;
    }

    public async Task<Budget> UpdateBudgetAsync(Budget budget)
    {
        _context.Entry(budget).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return budget;
    }

    public async Task<bool> DeleteBudgetAsync(int id)
    {
        var budget = await _context.Budgets.FindAsync(id);
        if (budget == null) return false;
        _context.Budgets.Remove(budget);
        await _context.SaveChangesAsync();
        return true;
    }
}
