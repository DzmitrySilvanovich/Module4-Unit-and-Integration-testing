using Microsoft.AspNetCore.Mvc;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Model;

namespace Ticketing.UI.Controllers
{
    /// <summary>
    /// Events API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Get a list with all "<see cref="EventReturnModel"/>" items.
        /// <returns>Events</returns>
        /// <response code="200">Return collection of events</response>
        /// <response code="204">Return empty collection</response>
        /// <response code="400">Bad request</response>        
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var events = await _eventService.GetEventsAsync();

            if (events is null)
            {
                return BadRequest(string.Empty);
            }
            else if (!events.Any())
            {
                return NoContent();
            }

            return Ok(events);
        }

        /// <summary>
        /// Get a list with all "<see cref="SeatReturnModel"/>" items.
        /// <returns>Collection of set from section for event</returns>
        /// <param name="eventId">Event id</param>
        /// <param name="sectionId">Section id</param>
        /// <response code="200">Return collection of seats</response>
        /// <response code="204">Return empty collection</response>
        /// <response code="400">Bad request</response>        
        /// </summary>
        [HttpGet("{eventId}/sections/{sectionId}/seats")]
        public async Task<IActionResult> GetSeatsAsync(int eventId, int sectionId)
        {
            var seats = await _eventService.GetSeatsAsync( eventId, sectionId);

            if (seats is null)
            {
                return BadRequest(string.Empty);
            }
            else if (!seats.Any())
            {
                return NoContent();
            }

            return Ok(seats);
        }
    }
}
