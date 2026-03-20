using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public class RecurringTransactionService : IRecurringTransactionService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public RecurringTransactionService(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<List<RecurringTransaction>> GetRecurringTransactionsAsync(string userId) =>
        await _context.RecurringTransactions.Where(r => r.UserId == userId).ToListAsync();

    public async Task<RecurringTransaction?> GetByIdAsync(int id) =>
        await _context.RecurringTransactions.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

    public async Task<RecurringTransaction> CreateAsync(RecurringTransaction transaction)
    {
        _context.RecurringTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<RecurringTransaction> UpdateAsync(RecurringTransaction transaction)
    {
        _context.Entry(transaction).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var transaction = await _context.RecurringTransactions.FindAsync(id);
        if (transaction == null) return false;
        _context.RecurringTransactions.Remove(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task ProcessDueTransactionsAsync()
    {
        var dueTransactions = await _context.RecurringTransactions
            .Where(r => r.IsActive && r.NextRunAt <= DateTime.UtcNow)
            .ToListAsync();

        foreach (var rt in dueTransactions)
        {
            if (rt.Type == RecurringTransactionType.Expense)
            {
                _context.Expenses.Add(new Expense
                {
                    Title = rt.Title,
                    Description = rt.Description,
                    Amount = rt.Amount,
                    Category = rt.ExpenseCategory ?? ExpenseCategory.Other,
                    Date = DateTime.UtcNow,
                    UserId = rt.UserId,
                    Currency = rt.Currency,
                    ExchangeRate = 1m,
                    AmountInBaseCurrency = rt.Amount,
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                _context.Incomes.Add(new Income
                {
                    Title = rt.Title,
                    Description = rt.Description,
                    Amount = rt.Amount,
                    Source = rt.IncomeSource ?? IncomeSource.Other,
                    Date = DateTime.UtcNow,
                    UserId = rt.UserId,
                    Currency = rt.Currency,
                    ExchangeRate = 1m,
                    AmountInBaseCurrency = rt.Amount,
                    CreatedAt = DateTime.UtcNow
                });
            }

            rt.LastRunAt = DateTime.UtcNow;
            rt.NextRunAt = rt.Frequency switch
            {
                RecurrenceFrequency.Daily => rt.NextRunAt.AddDays(1),
                RecurrenceFrequency.Weekly => rt.NextRunAt.AddDays(7),
                RecurrenceFrequency.Monthly => rt.NextRunAt.AddMonths(1),
                RecurrenceFrequency.Yearly => rt.NextRunAt.AddYears(1),
                _ => rt.NextRunAt.AddMonths(1)
            };

            if (rt.EndDate.HasValue && rt.NextRunAt > rt.EndDate.Value)
                rt.IsActive = false;

            if (!string.IsNullOrEmpty(rt.UserId))
                await _notificationService.CreateNotificationAsync(
                    rt.UserId,
                    NotificationType.RecurringTransactionCreated,
                    $"Recurring {rt.Type.ToString().ToLower()} '{rt.Title}' of {rt.Amount} {rt.Currency} has been processed.");
        }

        await _context.SaveChangesAsync();
    }
}
