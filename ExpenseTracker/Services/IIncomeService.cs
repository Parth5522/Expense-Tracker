using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface IIncomeService
{
    Task<List<Income>> GetAllIncomesAsync(string userId);
    Task<Income?> GetIncomeByIdAsync(int id);
    Task<Income> CreateIncomeAsync(Income income);
    Task<Income> UpdateIncomeAsync(Income income);
    Task<bool> DeleteIncomeAsync(int id);
}
