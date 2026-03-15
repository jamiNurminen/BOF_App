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
            [FromQuery] DateTime endDate)
        {
            if (startDate == default || endDate == default)
            {
                return BadRequest("Both startDate and endDate are required.");
            }

            if (startDate > endDate)
            {
                return BadRequest("Start date must be before end date.");
            }

            if (endDate > DateTime.Now)
            {
                return BadRequest("End date cannot be in the future.");
            }

            var result = await _service.GetBanknotesAsync(startDate, endDate);
            return Ok(new {quantity = result.Quantity, amount = result.Amount, startTime = result.startTime, endTime = result.endTime});
        }
    }
}