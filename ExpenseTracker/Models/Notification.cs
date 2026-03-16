using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models;

public enum NotificationType
{
    BudgetExceeded,
    RecurringTransactionCreated,
    GoalAchieved,
    UpcomingBill,
    General
}

public class Notification
{
    public int Id { get; set; }

    [Required]
    public NotificationType Type { get; set; }

    [Required]
    [StringLength(500)]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
}
