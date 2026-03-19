using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models;

public class Budget
{
    public int Id { get; set; }

    [Required]
    public int Month { get; set; }

    [Required]
    public int Year { get; set; }

    public ExpenseCategory? Category { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [StringLength(3)]
    public string Currency { get; set; } = "USD";

    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}
