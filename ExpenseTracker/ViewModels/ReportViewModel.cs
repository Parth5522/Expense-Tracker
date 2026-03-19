namespace ExpenseTracker.ViewModels;

public class MonthlyBreakdown
{
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal Expenses { get; set; }
    public decimal Income { get; set; }
    public decimal NetSavings => Income - Expenses;
}

public class ReportViewModel
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal NetSavings => TotalIncome - TotalExpenses;
    public Dictionary<string, decimal> ExpensesByCategory { get; set; } = new();
    public Dictionary<string, decimal> IncomeBySource { get; set; } = new();

    // Monthly/Yearly overview
    public int? SelectedYear { get; set; }
    public List<MonthlyBreakdown> MonthlyBreakdowns { get; set; } = new();
    public List<int> AvailableYears { get; set; } = new();
}
