using Microsoft.AspNetCore.Mvc;

namespace BOF_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BanknotesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetBanknotes(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string[]? currencies = null)
        {
            var selectedCurrencies = currencies ?? Array.Empty<string>();

            return Ok(new {quantity = 100, amount = 950.25, currencies = selectedCurrencies});
        }

    }
}