namespace BOF_app.Models
{
    public class BofExchangeRateResponse
    {
        public required List<ExchangeRate> ExchangeRates { get; set; }
        public required string Currency { get; set; }
        public required string CurrencyDenom { get; set; }
        public required string CurrencyNameFi { get; set; }
        public required string CurrencyNameSe { get; set; }
        public required string CurrencyNameEn { get; set; }
        public required string ECBPublished { get; set; }
    }

    public class ExchangeRate
    {
        public required string ObservationDate { get; set; }
        public required string Value { get; set; }
    }
}