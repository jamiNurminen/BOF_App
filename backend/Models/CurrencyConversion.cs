using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BOF_app.Models
{
    public class CurrencyConversion
    {
        public required string Currency { get; set; }
        public required decimal Amount { get; set; }
        public required decimal ExchangeRate { get; set; }
        public required string CurrencyDenom { get; set; }
        public required string CurrencyNameFi { get; set; }
        public required string CurrencyNameEn { get; set; }
        public required string CurrencyNameSe { get; set; }
        public required bool ECBPublished { get; set; }
    }
}