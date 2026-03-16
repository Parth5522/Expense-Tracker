using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models;

public class Tag
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(7)]
    public string Color { get; set; } = "#6c757d";

    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public ICollection<ExpenseTag> ExpenseTags { get; set; } = new List<ExpenseTag>();
    public ICollection<IncomeTag> IncomeTags { get; set; } = new List<IncomeTag>();
}

public class ExpenseTag
{
    public int ExpenseId { get; set; }
    public Expense? Expense { get; set; }
    public int TagId { get; set; }
    public Tag? Tag { get; set; }
}

public class IncomeTag
{
    public int IncomeId { get; set; }
    public Income? Income { get; set; }
    public int TagId { get; set; }
    public Tag? Tag { get; set; }
}
