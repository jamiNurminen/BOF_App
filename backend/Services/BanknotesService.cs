using System.Numerics;
using System.Runtime.InteropServices;
using BOF_app.Models;

namespace BOF_app.Services
{
    public class BanknotesService : IBanknotesService
    {
        public async Task<BanknotesData> GetBanknotesAsync(DateTime startDate, DateTime endDate)
        {
            // Simulate asynchronous operation
            await Task.Delay(100);

            // Return dummy data for demonstration purposes
            return new BanknotesData
            {
                startTime = startDate,
                endTime = endDate,
                Quantity = 100,
                Amount = 950.25m,
            };
        }
    }
}