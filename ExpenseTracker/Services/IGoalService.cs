using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface IGoalService
{
    Task<List<Goal>> GetGoalsAsync(string userId);
    Task<Goal?> GetGoalByIdAsync(int id);
    Task<Goal> CreateGoalAsync(Goal goal);
    Task<Goal> UpdateGoalAsync(Goal goal);
    Task<bool> DeleteGoalAsync(int id);
}
