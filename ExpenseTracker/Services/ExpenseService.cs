using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ApplicationDbContext _context;

        public ExpenseService(ApplicationDbContext context)
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
            return await _context.Expenses.Include(e => e.ExpenseTags).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<ExpenseFilterViewModel> GetFilteredExpensesAsync(ExpenseFilterViewModel filter)
        {
            var query = _context.Expenses.Include(e => e.ExpenseTags).AsQueryable();

            if (!string.IsNullOrEmpty(filter.UserId))
                query = query.Where(e => e.UserId == filter.UserId);

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
            return await GetDashboardDataAsync(string.Empty);
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync(string userId)
        {
            var query = _context.Expenses.AsQueryable();
            if (!string.IsNullOrEmpty(userId))
                query = query.Where(e => e.UserId == userId);

            var allExpenses = await query.ToListAsync();
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var monthlyExpenses = allExpenses
                .Where(e => e.Date.Month == currentMonth && e.Date.Year == currentYear)
                .Sum(e => e.Amount);

            var dailyGroups = allExpenses
                .Where(e => e.Date.Month == currentMonth && e.Date.Year == currentYear)
                .GroupBy(e => e.Date.Day)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

            var daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);

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
                CurrentMonthDaily = Enumerable.Range(1, daysInMonth)
                    .ToDictionary(
                        day => day.ToString(),
                        day => dailyGroups.TryGetValue(day, out var amount) ? amount : 0m)
            };

            return dashboard;
        }

        public async Task<decimal> GetSpentAmountAsync(string userId, int month, int year, ExpenseCategory? category)
        {
            var query = _context.Expenses.Where(e => e.UserId == userId && e.Date.Month == month && e.Date.Year == year);
            if (category.HasValue)
                query = query.Where(e => e.Category == category.Value);
            return await query.SumAsync(e => e.AmountInBaseCurrency);
        }

        public async Task SetTagsAsync(int expenseId, int[] tagIds)
        {
            var existing = await _context.ExpenseTags.Where(et => et.ExpenseId == expenseId).ToListAsync();
            _context.ExpenseTags.RemoveRange(existing);
            foreach (var tagId in tagIds)
                _context.ExpenseTags.Add(new ExpenseTag { ExpenseId = expenseId, TagId = tagId });
            await _context.SaveChangesAsync();
        }
    }
}
