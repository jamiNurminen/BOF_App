using BOF_app.Models;

namespace BOF_app.Services
{
    public interface IExchangeRatesService
    {
        Task<CurrencyConversion[]> GetExchangeRatesAsync(string[] currencies, decimal amount);
        Task<CurrencyInfo[]> GetAvailableCurrenciesAsync();
    }
}