using ExpenseTracker.Models;

namespace ExpenseTracker.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalExpenses { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public decimal AverageExpense { get; set; }
        public int TransactionCount { get; set; }
        public List<Expense> RecentTransactions { get; set; } = new();
        public Dictionary<ExpenseCategory, decimal> ExpensesByCategory { get; set; } = new();
        public Dictionary<string, decimal> MonthlyTrends { get; set; } = new();
    }
}
