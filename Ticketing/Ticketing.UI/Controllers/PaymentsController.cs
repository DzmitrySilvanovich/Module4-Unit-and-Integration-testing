using Microsoft.AspNetCore.Mvc;
using Ticketing.BAL.Contracts;

namespace Ticketing.UI.Controllers
{
    /// <summary>
    /// Payments API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Get the status of the payment.
        /// <param name="id">Payment id</param>
        /// <returns>payment status of payment</returns>
        /// <response code="200">Return a status of payment</response>
        /// <response code="400">Bad request</response>        
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var res = await _paymentService.GetPaymentStatusAsync(id);

            if (res is null)
            {
                return BadRequest(string.Empty);
            }
         
            return Ok(res);
        }

        /// <summary>
        /// Complete Payment.
        /// <param name="id">Payment id</param>
        /// <returns>payment status of payment</returns>
        /// <response code="204"></response>
        /// </summary>
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> PutCompleteAsync(int id)
        {
            var res = await _paymentService.CompletePaymentAsync(id);

            if (!res)
            {
                return BadRequest(res);
            }

            return Ok(res);
        }

        /// <summary>
        /// Fail Payment.
        /// <param name="id">Payment id</param>
        /// <returns>payment status of payment</returns>
        /// <response code="204"></response>
        /// </summary>
        [HttpPut("{id}/failed")]
        public async Task<IActionResult> PutFailedAsync(int id)
        {
            var res = await _paymentService.FailPaymentAsync(id);

            if (!res)
            {
                return BadRequest(res);
            }

            return Ok(res);
        }
    }
}
