namespace BOF_app.Models
{
    public sealed class BanknotesData
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long QuantityChange { get; set; }
        public decimal AmountChange { get; set; }
        public List<BanknoteBreakdown> Breakdown { get; set; } = [];
    }

    public sealed class BanknoteBreakdown
    {
        public int Denomination { get; set; }
        public long Quantity { get; set; }
        public decimal Amount { get; set; }
    }
}