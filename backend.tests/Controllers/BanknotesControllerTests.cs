using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using BOF_app.Controllers;
using BOF_app.Services;
using BOF_app.Models;

namespace BOF_app.Tests.Controllers
{
    public class BanknotesControllerTests
    {
        private readonly Mock<IBanknotesService> _mockService;
        private readonly BanknotesController _controller;

        public BanknotesControllerTests()
        {
            _mockService = new Mock<IBanknotesService>();
            _controller = new BanknotesController(_mockService.Object);
        }

        [Fact]
        public async Task GetBanknotes_WithValidDates_ReturnsOkWithData()
        {
            // Arrange
            var startDate = new DateTime(2025, 1, 1);
            var endDate = new DateTime(2025, 12, 31);
            var mockData = new BanknotesData
            {
                StartDate = startDate,
                EndDate = endDate,
                QuantityChange = 1000,
                AmountChange = 50000,
                Breakdown = new List<BanknoteBreakdown>()
            };

            _mockService.Setup(s => s.GetBanknotesAsync(startDate, endDate))
                .ReturnsAsync(mockData);

            // Act
            var result = await _controller.GetBanknotes(startDate, endDate);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnValue = okResult.Value.Should().BeOfType<BanknotesData>().Subject;
            returnValue.QuantityChange.Should().Be(1000);
            returnValue.AmountChange.Should().Be(50000);
            _mockService.Verify(s => s.GetBanknotesAsync(startDate, endDate), Times.Once);
        }

        [Fact]
        public async Task GetBanknotes_WithDefaultStartDate_ReturnsBadRequest()
        {
            // Arrange
            var startDate = default(DateTime);
            var endDate = new DateTime(2025, 12, 31);

            // Act
            var result = await _controller.GetBanknotes(startDate, endDate);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Both startDate and endDate are required.");
            _mockService.Verify(s => s.GetBanknotesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task GetBanknotes_WithDefaultEndDate_ReturnsBadRequest()
        {
            // Arrange
            var startDate = new DateTime(2025, 1, 1);
            var endDate = default(DateTime);

            // Act
            var result = await _controller.GetBanknotes(startDate, endDate);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Both startDate and endDate are required.");
        }

        [Fact]
        public async Task GetBanknotes_WithStartDateAfterEndDate_ReturnsBadRequest()
        {
            // Arrange
            var startDate = new DateTime(2025, 12, 31);
            var endDate = new DateTime(2025, 1, 1);

            // Act
            var result = await _controller.GetBanknotes(startDate, endDate);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Start date must be before end date.");
        }

        [Fact]
        public async Task GetBanknotes_WithFutureEndDate_ReturnsBadRequest()
        {
            // Arrange
            var startDate = new DateTime(2025, 1, 1);
            var endDate = DateTime.Now.AddDays(1);

            // Act
            var result = await _controller.GetBanknotes(startDate, endDate);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("End date cannot be in the future.");
        }

        [Fact]
        public async Task GetBanknotes_WithValidDatesAndBreakdown_ReturnsDataWithBreakdown()
        {
            // Arrange
            var startDate = new DateTime(2025, 1, 1);
            var endDate = new DateTime(2025, 12, 31);
            var breakdown = new List<BanknoteBreakdown>
            {
                new BanknoteBreakdown { Denomination = 500, Quantity = 100, Amount = 50000 },
                new BanknoteBreakdown { Denomination = 100, Quantity = 200, Amount = 20000 }
            };
            var mockData = new BanknotesData
            {
                StartDate = startDate,
                EndDate = endDate,
                QuantityChange = 300,
                AmountChange = 70000,
                Breakdown = breakdown
            };

            _mockService.Setup(s => s.GetBanknotesAsync(startDate, endDate))
                .ReturnsAsync(mockData);

            // Act
            var result = await _controller.GetBanknotes(startDate, endDate);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnValue = okResult.Value.Should().BeOfType<BanknotesData>().Subject;
            returnValue.Breakdown.Should().HaveCount(2);
            returnValue.Breakdown.First().Denomination.Should().Be(500);
        }
    }
}
