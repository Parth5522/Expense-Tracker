using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface IRecurringTransactionService
{
    Task<List<RecurringTransaction>> GetRecurringTransactionsAsync(string userId);
    Task<RecurringTransaction?> GetByIdAsync(int id);
    Task<RecurringTransaction> CreateAsync(RecurringTransaction transaction);
    Task<RecurringTransaction> UpdateAsync(RecurringTransaction transaction);
    Task<bool> DeleteAsync(int id);
    Task ProcessDueTransactionsAsync();
}
