namespace BOF_app.Models
{
    public class BanknotesData
    {
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public List<CurrencyConversion> Conversions { get; set; } = new List<CurrencyConversion>();
    }

    public class CurrencyConversion
    {
        public required string Currency { get; set; }
        public decimal Amount { get; set; }
    }
}