using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public class CurrencyService : ICurrencyService
{
    private readonly ApplicationDbContext _context;

    public CurrencyService(ApplicationDbContext context) => _context = context;

    public async Task<List<Currency>> GetAllCurrenciesAsync() =>
        await _context.Currencies.OrderBy(c => c.Code).ToListAsync();

    public async Task<Currency?> GetCurrencyAsync(string code) =>
        await _context.Currencies.FindAsync(code);

    public async Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency)
    {
        if (fromCurrency == toCurrency) return amount;

        var from = await _context.Currencies.FindAsync(fromCurrency);
        var to = await _context.Currencies.FindAsync(toCurrency);

        if (from == null || to == null) return amount;

        var amountInUsd = amount / from.RateToUsd;
        return amountInUsd * to.RateToUsd;
    }
}
