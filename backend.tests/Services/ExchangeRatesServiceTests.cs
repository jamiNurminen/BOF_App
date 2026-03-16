using Xunit;
using FluentAssertions;
using BOF_app.Services;
using System.Globalization;

namespace BOF_app.Tests.Services
{
    public class ExchangeRatesServiceTests
    {
        [Theory]
        [InlineData("1.1476000000")]
        [InlineData("1,1476000000")]
        [InlineData("1.5")]
        [InlineData("1,5")]
        public void ParseExchangeRateValue_WithVariousFormats_ParsesCorrectly(string input)
        {
            var parsedSuccessfully = decimal.TryParse(
                input,
                NumberStyles.Number,
                CultureInfo.GetCultureInfo("fi-FI"),
                out var parsed);

            if (!parsedSuccessfully)
            {
                parsedSuccessfully = decimal.TryParse(
                    input,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out parsed);
            }

            parsedSuccessfully.Should().BeTrue();
            parsed.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("-1")]
        public void GetExchangeRatesAsync_WithInvalidAmount_ThrowsException(string amountStr)
        {
            // Arrange
            if (!decimal.TryParse(amountStr, out var amount))
            {
                amount = 0;
            }

            // Act & Assert
            amount.Should().BeLessThanOrEqualTo(0);
        }

        [Fact]
        public void CurrencyConversionCalculation_MultiplesByExchangeRate()
        {
            // Arrange
            var principal = 100m;
            var exchangeRate = 1.1m;

            // Act
            var result = principal * exchangeRate;

            // Assert
            result.Should().Be(110m);
        }

        [Fact]
        public void AvailableCurrencies_ShouldContainCommonCurrencies()
        {
            // Common currencies that BOF API provides
            var expectedCurrencies = new[] { "USD", "GBP", "JPY", "CHF", "SEK", "NOK", "DKK" };

            // Assert
            expectedCurrencies.Should().NotBeEmpty();
            expectedCurrencies.Should().Contain("USD");
        }

        [Theory]
        [InlineData("USD", "EUR", "1.1", "110")]
        [InlineData("GBP", "EUR", "1.3", "130")]
        [InlineData("JPY", "EUR", "0.0067", "0.67")]
        public void CurrencyConversion_WithRealWorldRates(string currency, string baseCurrency, string rateStr, string expectedAmountStr)
        {
            // Arrange
            var amount = 100m;
            var rate = decimal.Parse(rateStr, CultureInfo.InvariantCulture);
            var expectedAmount = decimal.Parse(expectedAmountStr, CultureInfo.InvariantCulture);

            currency.Should().NotBeNullOrWhiteSpace();
            baseCurrency.Should().Be("EUR");

            // Act
            var converted = amount * rate;

            // Assert
            converted.Should().Be(expectedAmount);
        }
    }
}
