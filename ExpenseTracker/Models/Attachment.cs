using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models;

public class Attachment
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [StringLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public int? ExpenseId { get; set; }
    public Expense? Expense { get; set; }

    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}
