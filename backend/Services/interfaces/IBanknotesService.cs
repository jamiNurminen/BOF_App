using BOF_app.Models;

namespace BOF_app.Services
{
    public interface IBanknotesService
    {
        Task<BanknotesData> GetBanknotesAsync(DateTime startDate, DateTime endDate, string[]? currencies = null);
    }
}