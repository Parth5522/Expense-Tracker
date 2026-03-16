using CsvHelper;
using CsvHelper.Configuration;
using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ExpenseTracker.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;

    public ReportService(ApplicationDbContext context) => _context = context;

    public async Task<byte[]> ExportExpensesToCsvAsync(string userId, DateTime? from, DateTime? to)
    {
        var query = _context.Expenses.Where(e => e.UserId == userId);
        if (from.HasValue) query = query.Where(e => e.Date >= from.Value);
        if (to.HasValue) query = query.Where(e => e.Date <= to.Value);
        var expenses = await query.OrderByDescending(e => e.Date).ToListAsync();

        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

        csv.WriteRecords(expenses.Select(e => new
        {
            e.Id,
            e.Title,
            e.Description,
            e.Amount,
            e.Currency,
            Category = e.Category.ToString(),
            Date = e.Date.ToString("yyyy-MM-dd"),
            e.AmountInBaseCurrency
        }));

        await writer.FlushAsync();
        return memoryStream.ToArray();
    }

    public async Task<byte[]> ExportIncomesToCsvAsync(string userId, DateTime? from, DateTime? to)
    {
        var query = _context.Incomes.Where(i => i.UserId == userId);
        if (from.HasValue) query = query.Where(i => i.Date >= from.Value);
        if (to.HasValue) query = query.Where(i => i.Date <= to.Value);
        var incomes = await query.OrderByDescending(i => i.Date).ToListAsync();

        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

        csv.WriteRecords(incomes.Select(i => new
        {
            i.Id,
            i.Title,
            i.Description,
            i.Amount,
            i.Currency,
            Source = i.Source.ToString(),
            Date = i.Date.ToString("yyyy-MM-dd"),
            i.AmountInBaseCurrency
        }));

        await writer.FlushAsync();
        return memoryStream.ToArray();
    }
}
