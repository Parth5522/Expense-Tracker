using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models;

public enum IncomeSource
{
    Salary,
    Freelance,
    Investment,
    Rental,
    Business,
    Gift,
    Bonus,
    Other
}

public class Income
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    public IncomeSource Source { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(3)]
    public string Currency { get; set; } = "USD";

    [Column(TypeName = "decimal(18,6)")]
    public decimal ExchangeRate { get; set; } = 1m;

    [Column(TypeName = "decimal(18,2)")]
    public decimal AmountInBaseCurrency { get; set; }

    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public ICollection<IncomeTag> IncomeTags { get; set; } = new List<IncomeTag>();
}
