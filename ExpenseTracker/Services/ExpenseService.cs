using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ExpenseDbContext _context;

        public ExpenseService(ExpenseDbContext context)
        {
            _context = context;
        }

        public async Task<List<Expense>> GetAllExpensesAsync()
        {
            return await _context.Expenses
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<Expense?> GetExpenseByIdAsync(int id)
        {
            return await _context.Expenses.FindAsync(id);
        }

        public async Task<ExpenseFilterViewModel> GetFilteredExpensesAsync(ExpenseFilterViewModel filter)
        {
            var query = _context.Expenses.AsQueryable();

            // Apply filters
            if (filter.FromDate.HasValue)
            {
                var fromDate = filter.FromDate.Value.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(filter.FromDate.Value, DateTimeKind.Utc) 
                    : filter.FromDate.Value;
                query = query.Where(e => e.Date >= fromDate);
            }

            if (filter.ToDate.HasValue)
            {
                var toDate = filter.ToDate.Value.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(filter.ToDate.Value, DateTimeKind.Utc) 
                    : filter.ToDate.Value;
                query = query.Where(e => e.Date <= toDate);
            }

            if (filter.Category.HasValue)
            {
                query = query.Where(e => e.Category == filter.Category.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(e => 
                    e.Title.ToLower().Contains(searchTerm) || 
                    (e.Description != null && e.Description.ToLower().Contains(searchTerm)));
            }

            // Get total count for pagination
            filter.TotalItems = await query.CountAsync();
            filter.TotalPages = (int)Math.Ceiling(filter.TotalItems / (double)filter.PageSize);

            // Apply pagination
            filter.Expenses = await query
                .OrderByDescending(e => e.Date)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return filter;
        }

        public async Task<Expense> CreateExpenseAsync(Expense expense)
        {
            expense.CreatedAt = DateTime.UtcNow;
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return expense;
        }

        public async Task<Expense> UpdateExpenseAsync(Expense expense)
        {
            _context.Entry(expense).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return expense;
        }

        public async Task<bool> DeleteExpenseAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return false;
            }

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var allExpenses = await _context.Expenses.ToListAsync();
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var monthlyExpenses = allExpenses
                .Where(e => e.Date.Month == currentMonth && e.Date.Year == currentYear)
                .Sum(e => e.Amount);

            var dashboard = new DashboardViewModel
            {
                TotalExpenses = allExpenses.Sum(e => e.Amount),
                MonthlyExpenses = monthlyExpenses,
                AverageExpense = allExpenses.Any() ? allExpenses.Average(e => e.Amount) : 0,
                TransactionCount = allExpenses.Count,
                RecentTransactions = allExpenses
                    .OrderByDescending(e => e.Date)
                    .Take(5)
                    .ToList(),
                ExpensesByCategory = allExpenses
                    .GroupBy(e => e.Category)
                    .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount)),
                MonthlyTrends = allExpenses
                    .Where(e => e.Date >= DateTime.Now.AddMonths(-6))
                    .GroupBy(e => new { e.Date.Year, e.Date.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                    .ToDictionary(
                        g => $"{new DateTime(g.Key.Year, g.Key.Month, 1):MMM yyyy}",
                        g => g.Sum(e => e.Amount))
            };

            return dashboard;
        }
    }
}
