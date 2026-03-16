using ExpenseTracker.Models;
using ExpenseTracker.ViewModels;

namespace ExpenseTracker.Services
{
    public interface IExpenseService
    {
        Task<List<Expense>> GetAllExpensesAsync();
        Task<Expense?> GetExpenseByIdAsync(int id);
        Task<ExpenseFilterViewModel> GetFilteredExpensesAsync(ExpenseFilterViewModel filter);
        Task<Expense> CreateExpenseAsync(Expense expense);
        Task<Expense> UpdateExpenseAsync(Expense expense);
        Task<bool> DeleteExpenseAsync(int id);
        Task<DashboardViewModel> GetDashboardDataAsync();
        Task<DashboardViewModel> GetDashboardDataAsync(string userId);
        Task<decimal> GetSpentAmountAsync(string userId, int month, int year, ExpenseCategory? category);
        Task SetTagsAsync(int expenseId, int[] tagIds);
    }
}
