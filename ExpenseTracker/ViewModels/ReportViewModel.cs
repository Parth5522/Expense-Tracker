namespace ExpenseTracker.ViewModels;

public class ReportViewModel
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal NetSavings => TotalIncome - TotalExpenses;
    public Dictionary<string, decimal> ExpensesByCategory { get; set; } = new();
    public Dictionary<string, decimal> IncomeBySource { get; set; } = new();
}
