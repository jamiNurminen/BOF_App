using Microsoft.AspNetCore.Mvc;
using BOF_app.Services;

namespace BOF_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BanknotesController : ControllerBase
    {
        private readonly IBanknotesService _service;

        public BanknotesController(IBanknotesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetBanknotes(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string[]? currencies = null)
        {
            var result = await _service.GetBanknotesAsync(startDate, endDate, currencies);
            return Ok(new {quantity = result.Quantity, amount = result.Amount, currencies = result.Conversions});
        }

    }
}