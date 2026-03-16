using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models;

public enum RecurrenceFrequency
{
    Daily,
    Weekly,
    Monthly,
    Yearly
}

public enum RecurringTransactionType
{
    Expense,
    Income
}

public class RecurringTransaction
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public RecurringTransactionType Type { get; set; }

    public ExpenseCategory? ExpenseCategory { get; set; }
    public IncomeSource? IncomeSource { get; set; }

    [Required]
    public RecurrenceFrequency Frequency { get; set; }

    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public DateTime NextRunAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastRunAt { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(3)]
    public string Currency { get; set; } = "USD";

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
}
