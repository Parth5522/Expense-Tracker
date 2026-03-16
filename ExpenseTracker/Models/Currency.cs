using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models;

public class Currency
{
    [Key]
    [StringLength(3)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(5)]
    public string Symbol { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,6)")]
    public decimal RateToUsd { get; set; } = 1m;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
