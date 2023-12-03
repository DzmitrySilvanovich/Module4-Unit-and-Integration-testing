using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Services;

namespace Ticketing.UI.Controllers
{
    /// <summary>
    /// Orders API
    /// </summary>
    //  [Route("api/[controller]")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Release Carts From Order.
        /// <param name="orderId">Cart id</param>
        /// <response code="200">Return a status of request</response>
        /// </summary>
        [HttpDelete("release/{orderId}")]
        public async Task<IActionResult> DeleteAsync(int orderId)
        {
            var result = await _orderService.ReleaseCartsFromOrderAsync(orderId);


            if (!result)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
