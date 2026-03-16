namespace ExpenseTracker.Services;

public interface IReportService
{
    Task<byte[]> ExportExpensesToCsvAsync(string userId, DateTime? from, DateTime? to);
    Task<byte[]> ExportIncomesToCsvAsync(string userId, DateTime? from, DateTime? to);
}
