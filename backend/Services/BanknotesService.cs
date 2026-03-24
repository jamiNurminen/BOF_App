using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BOF_app.Models;

namespace BOF_app.Services
{
    public class BanknotesService : IBanknotesService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.boffsaopendata.fi/v4/observations/BOF_BKN1_PUBL";

        public BanknotesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task<BanknotesData> GetBanknotesAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var url = $"{BaseUrl}?startPeriod={Uri.EscapeDataString(startDate.ToString("M/d/yyyy"))}&endPeriod={Uri.EscapeDataString(endDate.ToString("M/d/yyyy"))}&pageNumber=1&pageSize=20";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var bofResponse = await response.Content.ReadFromJsonAsync<BofBanknoteResponseDto>();


                if (bofResponse == null)
                {
                    throw new Exception("Failed to parse banknotes data from BOF API.");
                }

                var byDenomination = new Dictionary<int, BanknoteBreakdown>();
                var hasPv = new HashSet<int>();

                foreach (var item in bofResponse.Items)
                {
                    var (denomination, unit) = ParseSeriesName(item.Name);

                        var ordered = item.Observations.OrderBy(o => o.Period).ToList(); 
                        if (ordered.Count < 2) continue;
                        
                        var change = ordered[^1].Value + ordered[0].Value;

                    if (!byDenomination.TryGetValue(denomination, out var row))
                        row = new BanknoteBreakdown { Denomination = denomination };

                    if (unit == "PN") row.Quantity += change;
                    if (unit == "PV") { row.Amount += change; hasPv.Add(denomination); }
                    byDenomination[denomination] = row;
                }

                foreach (var row in byDenomination.Values.Where(x => !hasPv.Contains(x.Denomination)))
                    row.Amount = row.Quantity * row.Denomination; // If PV data is missing, calculate it fron PN and denomination

                var breakdown = byDenomination.Values.OrderByDescending(x => x.Denomination).ToList();

                var banknotesData = new BanknotesData
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    QuantityChange = breakdown.Sum(x => x.Quantity),
                    AmountChange = breakdown.Sum(x => x.Amount),
                    Breakdown = breakdown
                };

                return banknotesData;
            } catch (Exception ex)
            {
                throw new Exception("Failed to fetch banknotes data from BOF API.", ex);
            }
        }

        private static(int Denomination, string Unit) ParseSeriesName(string name)
        {
            var parts = name.Split('.');
            var denomPart = parts[5];
            var unitPart = parts[7];

            if (denomPart is null || !int.TryParse(denomPart[1..], out var denomination))
                throw new FormatException($"Cannot parse denomination from '{name}'.");
            if (unitPart is null)
                throw new FormatException($"Cannot parse unit from '{name}'.");

            return (denomination, unitPart);
        }
    }
}