using Microsoft.AspNetCore.Mvc;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Model;

namespace Ticketing.UI.Controllers
{
    /// <summary>
    /// Carts API
    /// </summary>
    [Route("api/[controller]/carts")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;

        }

        /// <summary>
        /// Get items of the cart.
        /// <param name="id">Cart id</param>
        /// <returns>The collection of items of the cart</returns>
        /// <response code="200">Return a status of request</response>
        /// <response code="400">Bad request</response>
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var shoppingCarts = await _cartService.CartItemsAsync(id);

            if (shoppingCarts is null)
            {
                return BadRequest(string.Empty);
            }
            else if (!shoppingCarts.Any())
            {
                return NoContent();
            }

            return Ok(shoppingCarts);
        }

        /// <summary>
        /// Add Seat To Cart.
        /// <param name="id">Cart id</param>
        /// <param name="orderCartModel">Payload for the action</param>
        /// <returns>The collection of items of the cart</returns>
        /// <response code="200">Return a status of request with result</response>
        /// <response code="400">Bad request</response>        
        /// </summary>
        [HttpPost("{id}")]
         public async Task<IActionResult> Post(Guid id, [FromBody] OrderCartModel orderCartModel)
        {
            var result = await _cartService.AddSeatToCartAsync(id, orderCartModel);

            if (result is null)
            {
                return BadRequest(string.Empty);
            }

            return Ok(result);
        }

        /// <summary>
        /// Book Seat To Cart.
        /// <param name="cartId">Cart id</param>
        /// <response code="200">Return a status of request</response>
        /// </summary>
        [HttpPut("{cartId}/book")]
        public async Task<IActionResult> PutAsync(Guid cartId)
        {
            var result = await _cartService.BookSeatToCartAsync(cartId);

            if(result == 0)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete Seat For Cart.
        /// <param name="cartId">Cart id</param>
        /// <param name="eventId">Event id</param>
        /// <param name="seatId">Seat id</param>
        /// <response code="200">Return a status of request</response>
        /// </summary>
        [HttpDelete("{cartId}/events/{eventId}/seats/{seatId}")]
        public async Task<IActionResult> DeleteAsync(Guid cartId, int eventId, int seatId)
        {
            var result = await _cartService.DeleteSeatForCartAsync(cartId, eventId, seatId);

            if (!result)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
