using Microsoft.AspNetCore.Mvc;
using BOF_app.Services;
using System.Runtime.CompilerServices;

namespace BOF_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly IExchangeRatesService _service;

        public ExchangeRatesController(IExchangeRatesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetExchangeRates(
            [FromQuery] string[] currencies,
            [FromQuery] decimal amount)
        {
            if (amount <= 0)
            {
                return BadRequest("Amount must be greater than zero.");
            }

            if (currencies == null || currencies.Length == 0)
            {
                return BadRequest("At least one currency is required.");
            }

            try
            {
                var result = await _service.GetExchangeRatesAsync(currencies, amount);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("No exchange rates found for the specified currencies.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

        }

        [HttpGet("currencies")]

        public async Task<IActionResult> GetCurrencies()
        {
            var result = await _service.GetAvailableCurrenciesAsync();
            return Ok(result);
        }
    }
}