using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using BOF_app.Controllers;
using BOF_app.Services;
using BOF_app.Models;

namespace BOF_app.Tests.Controllers
{
    public class ExchangeRatesControllerTests
    {
        private readonly Mock<IExchangeRatesService> _mockService;
        private readonly ExchangeRatesController _controller;

        public ExchangeRatesControllerTests()
        {
            _mockService = new Mock<IExchangeRatesService>();
            _controller = new ExchangeRatesController(_mockService.Object);
        }

        [Fact]
        public async Task GetExchangeRates_WithValidInput_ReturnsOkWithData()
        {
            // Arrange
            var currencies = new[] { "USD" };
            var amount = 100m;
            var mockData = new[]
            {
                new CurrencyConversion
                {
                    Currency = "USD",
                    ExchangeRate = 1.1m,
                    Amount = 110m,
                    CurrencyDenom = "EUR",
                    CurrencyNameFi = "Yhdysvaltain dollari",
                    CurrencyNameEn = "US Dollar",
                    CurrencyNameSe = "US dollar",
                    ECBPublished = true
                }
            };

            _mockService.Setup(s => s.GetExchangeRatesAsync(currencies, amount))
                .ReturnsAsync(mockData);

            // Act
            var result = await _controller.GetExchangeRates(currencies, amount);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnValue = okResult.Value.Should().BeOfType<CurrencyConversion[]>().Subject;
            returnValue.Should().HaveCount(1);
            returnValue.First().Currency.Should().Be("USD");
            returnValue.First().Amount.Should().Be(110m);
        }

        [Fact]
        public async Task GetExchangeRates_WithZeroAmount_ReturnsBadRequest()
        {
            // Arrange
            var currencies = new[] { "USD" };
            var amount = 0m;

            // Act
            var result = await _controller.GetExchangeRates(currencies, amount);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Amount must be greater than zero.");
        }

        [Fact]
        public async Task GetExchangeRates_WithNegativeAmount_ReturnsBadRequest()
        {
            // Arrange
            var currencies = new[] { "USD" };
            var amount = -50m;

            // Act
            var result = await _controller.GetExchangeRates(currencies, amount);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Amount must be greater than zero.");
        }

        [Fact]
        public async Task GetExchangeRates_WithNullCurrencies_ReturnsBadRequest()
        {
            // Arrange
            string[]? currencies = null;
            var amount = 100m;

            // Act
            var result = await _controller.GetExchangeRates(currencies!, amount);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("At least one currency is required.");
        }

        [Fact]
        public async Task GetExchangeRates_WithEmptyCurrencies_ReturnsBadRequest()
        {
            // Arrange
            var currencies = Array.Empty<string>();
            var amount = 100m;

            // Act
            var result = await _controller.GetExchangeRates(currencies, amount);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("At least one currency is required.");
        }

        [Fact]
        public async Task GetExchangeRates_WithKeyNotFoundException_ReturnsNotFound()
        {
            // Arrange
            var currencies = new[] { "INVALID" };
            var amount = 100m;

            _mockService.Setup(s => s.GetExchangeRatesAsync(currencies, amount))
                .ThrowsAsync(new KeyNotFoundException("No exchange rates found."));

            // Act
            var result = await _controller.GetExchangeRates(currencies, amount);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be("No exchange rates found for the specified currencies.");
        }

        [Fact]
        public async Task GetExchangeRates_WithGenericException_ReturnsInternalServerError()
        {
            // Arrange
            var currencies = new[] { "USD" };
            var amount = 100m;
            var exceptionMessage = "Database connection failed";

            _mockService.Setup(s => s.GetExchangeRatesAsync(currencies, amount))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetExchangeRates(currencies, amount);

            // Assert
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
            var responseMessage = statusCodeResult.Value.Should().BeOfType<string>().Subject;
            responseMessage.Should().Contain(exceptionMessage);
        }

        [Fact]
        public async Task GetCurrencies_ReturnsCurrencyList()
        {
            // Arrange
            var mockCurrencies = new[]
            {
                new CurrencyInfo
                {
                    Currency = "USD",
                    CurrencyDenom = "EUR",
                    CurrencyNameFi = "Yhdysvaltain dollari",
                    CurrencyNameEn = "US Dollar",
                    CurrencyNameSe = "US dollar"
                },
                new CurrencyInfo
                {
                    Currency = "GBP",
                    CurrencyDenom = "EUR",
                    CurrencyNameFi = "Punta",
                    CurrencyNameEn = "British pound",
                    CurrencyNameSe = "Brittiska pund"
                }
            };

            _mockService.Setup(s => s.GetAvailableCurrenciesAsync())
                .ReturnsAsync(mockCurrencies);

            // Act
            var result = await _controller.GetCurrencies();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnValue = okResult.Value.Should().BeOfType<CurrencyInfo[]>().Subject;
            returnValue.Should().HaveCount(2);
            returnValue.First().Currency.Should().Be("USD");
        }

        [Fact]
        public async Task GetExchangeRates_WithMultipleCurrencies_ReturnsAllConversions()
        {
            // Arrange
            var currencies = new[] { "USD", "GBP" };
            var amount = 100m;
            var mockData = new[]
            {
                new CurrencyConversion
                {
                    Currency = "USD",
                    ExchangeRate = 1.1m,
                    Amount = 110m,
                    CurrencyDenom = "EUR",
                    CurrencyNameFi = "Yhdysvaltain dollari",
                    CurrencyNameEn = "US Dollar",
                    CurrencyNameSe = "US dollar",
                    ECBPublished = true
                },
                new CurrencyConversion
                {
                    Currency = "GBP",
                    ExchangeRate = 1.3m,
                    Amount = 130m,
                    CurrencyDenom = "EUR",
                    CurrencyNameFi = "Punta",
                    CurrencyNameEn = "British pound",
                    CurrencyNameSe = "Brittiska pund",
                    ECBPublished = true
                }
            };

            _mockService.Setup(s => s.GetExchangeRatesAsync(currencies, amount))
                .ReturnsAsync(mockData);

            // Act
            var result = await _controller.GetExchangeRates(currencies, amount);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnValue = okResult.Value.Should().BeOfType<CurrencyConversion[]>().Subject;
            returnValue.Should().HaveCount(2);
        }
    }
}
