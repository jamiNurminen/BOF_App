using System.Numerics;
using System.Runtime.InteropServices;
using BOF_app.Models;

namespace BOF_app.Services
{
    public class BanknotesService : IBanknotesService
    {
        public async Task<BanknotesData> GetBanknotesAsync(DateTime startDate, DateTime endDate, string[]? currencies = null)
        {
            // Simulate asynchronous operation
            await Task.Delay(100);

            var selectedCurrencies = currencies ?? Array.Empty<string>();

            var exchangeRates = new Dictionary<string, decimal>
            {
                { "USD", 1.0m },
                { "EUR", 0.85m },
                { "GBP", 0.75m },
                { "JPY", 110.0m }
            };

            // Return dummy data for demonstration purposes
            return new BanknotesData
            {
                Quantity = 100,
                Amount = 950.25m,
                Conversions = selectedCurrencies.Select(c => new CurrencyConversion
                {
                    Currency = c,
                    Amount = 950.25m * (exchangeRates.ContainsKey(c) ? exchangeRates[c] : 1.0m) // Use exchange rate if available, otherwise default to 1.0
                }).ToList()
            };
        }
    }
}