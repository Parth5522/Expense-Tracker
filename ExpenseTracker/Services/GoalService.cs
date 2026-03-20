using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public class GoalService : IGoalService
{
    private readonly ApplicationDbContext _context;

    public GoalService(ApplicationDbContext context) => _context = context;

    public async Task<List<Goal>> GetGoalsAsync(string userId) =>
        await _context.Goals.Where(g => g.UserId == userId).ToListAsync();

    public async Task<Goal?> GetGoalByIdAsync(int id) =>
        await _context.Goals.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id);

    public async Task<Goal> CreateGoalAsync(Goal goal)
    {
        goal.CreatedAt = DateTime.UtcNow;
        _context.Goals.Add(goal);
        await _context.SaveChangesAsync();
        return goal;
    }

    public async Task<Goal> UpdateGoalAsync(Goal goal)
    {
        goal.UpdatedAt = DateTime.UtcNow;
        _context.Entry(goal).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return goal;
    }

    public async Task<bool> DeleteGoalAsync(int id)
    {
        var goal = await _context.Goals.FindAsync(id);
        if (goal == null) return false;
        _context.Goals.Remove(goal);
        await _context.SaveChangesAsync();
        return true;
    }
}
