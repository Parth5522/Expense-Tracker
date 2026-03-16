using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Models;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public string BaseCurrency { get; set; } = "USD";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<Income> Incomes { get; set; } = new List<Income>();
    public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    public ICollection<Goal> Goals { get; set; } = new List<Goal>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<RecurringTransaction> RecurringTransactions { get; set; } = new List<RecurringTransaction>();
}
