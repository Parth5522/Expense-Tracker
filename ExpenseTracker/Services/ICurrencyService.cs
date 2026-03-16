using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface ICurrencyService
{
    Task<List<Currency>> GetAllCurrenciesAsync();
    Task<Currency?> GetCurrencyAsync(string code);
    Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency);
}
