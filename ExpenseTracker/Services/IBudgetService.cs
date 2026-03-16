using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface IBudgetService
{
    Task<List<Budget>> GetBudgetsAsync(string userId, int month, int year);
    Task<Budget?> GetBudgetByIdAsync(int id);
    Task<Budget> CreateBudgetAsync(Budget budget);
    Task<Budget> UpdateBudgetAsync(Budget budget);
    Task<bool> DeleteBudgetAsync(int id);
}
