using ExpenseTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.ViewModels
{
    public class ExpenseFilterViewModel
    {
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public ExpenseCategory? Category { get; set; }

        public string? SearchTerm { get; set; }

        public string? UserId { get; set; }

        public List<Expense> Expenses { get; set; } = new();

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
}
