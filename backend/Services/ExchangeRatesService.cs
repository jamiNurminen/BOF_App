using System.Runtime.CompilerServices;
using System.Net.Http;
using BOF_app.Models;
using System.Net.Http.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;
using System.ComponentModel;
using Microsoft.VisualBasic;
using System.Data.SqlTypes;
using System.Linq.Expressions;
using System.Runtime;

namespace BOF_app.Services
{
    public class ExchangeRatesService : IExchangeRatesService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.boffsaopendata.fi/referencerates/v2/api/V2";

        public ExchangeRatesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CurrencyConversion[]> GetExchangeRatesAsync(string[] currencies, decimal amount)
        {
            try
            {
                var url = $"{BaseUrl}?currencies={string.Join(",", currencies)}";
                var response = await _httpClient.GetAsync(url);
                
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new KeyNotFoundException("No exchange rates found for the specified currencies.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"BOF API request failed with status code: {response.StatusCode}");
                }

                var bofRates = await response.Content.ReadFromJsonAsync<List<BofExchangeRateResponse>>();

                if (bofRates == null || bofRates.Count == 0)
                {
                    throw new KeyNotFoundException("No exchange rates found for the specified currencies.");
                }

                var exchangeRates = bofRates.Select(rate => 
                {
                    Console.WriteLine("Rate: " + rate);
                    var rate_value = Convert.ToDecimal(rate.ExchangeRates.FirstOrDefault()?.Value ?? "0");
            
                    return new CurrencyConversion
                    {
                        Currency = rate.Currency,
                        ExchangeRate = rate_value,
                        Amount = rate_value * amount,  // Calculating the converted amount
                        CurrencyDenom = rate.CurrencyDenom,
                        CurrencyNameFi = rate.CurrencyNameFi,
                        CurrencyNameEn = rate.CurrencyNameEn,
                        CurrencyNameSe = rate.CurrencyNameSe,
                        ECBPublished = rate.ECBPublished == "true"
                    };
                }).ToArray();   

                return exchangeRates;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"BOF API request failed: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred: {ex.Message}", ex);
            }
        }

        public async Task<CurrencyInfo[]> GetAvailableCurrenciesAsync()
        {
            try
            {
                var url = $"{BaseUrl}";
                Console.WriteLine("Fetching available currencies from BOF API: " + url);

                var response = await _httpClient.GetAsync(BaseUrl);
                
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new KeyNotFoundException("No currencies found.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"BOF API request failed with status code: {response.StatusCode}");
                }

                var currencies = await response.Content.ReadFromJsonAsync<List<BofExchangeRateResponse>>();

                if (currencies == null || currencies.Count == 0)
                {
                    throw new KeyNotFoundException("No currencies found.");
                }

                var currencyInfos = currencies.Select(rate => new CurrencyInfo
                {
                    Currency = rate.Currency,
                    CurrencyDenom = rate.CurrencyDenom,
                    CurrencyNameFi = rate.CurrencyNameFi,
                    CurrencyNameEn = rate.CurrencyNameEn,
                    CurrencyNameSe = rate.CurrencyNameSe
                }).ToArray();

                return currencyInfos;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"BOF API request failed: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
               throw new Exception($"An unexpected error occurred: {ex.Message}", ex); 
            }
        }
    }
}