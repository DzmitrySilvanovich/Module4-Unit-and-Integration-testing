using Microsoft.AspNetCore.Mvc;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Model;

namespace Ticketing.UI.Controllers
{
    /// <summary>
    /// Venues API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenuesController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        /// <summary>
        /// Get a list with all "<see cref="VenueReturnModel"/>" items.
        /// <returns>Venues</returns>
        /// <response code="200">Return collection of venues</response>
        /// <response code="204">Return empty collection</response>
        /// <response code="400">Bad request</response>        
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var venues = await _venueService.GetVenuesAsync();

            if (venues is null)
            {
                return BadRequest(string.Empty);
            }
            else if(!venues.Any())
            {
                return NoContent();
            }

            return Ok(venues);
        }

        /// <summary>
        /// Get a list with all "<see cref="VenueReturnModel"/>" items.
        /// <param name="venueId">Venue Id</param>
        /// <returns>Collection of section of venues</returns>
        /// <response code="200">Return collection of venues</response>
        /// <response code="204">Return empty collection</response>
        /// <response code="400">Bad request</response>        
        /// </summary>
        [HttpGet("{venueId}/sections")]
        public async Task<IActionResult> GetSectionsOfVenue(int venueId)
        {
            var sections = await _venueService.GetSectionsOfVenue(venueId);

            if (sections is null)
            {
                return BadRequest(string.Empty);
            }
            else if (!sections.Any())
            {
                return NoContent();
            }

            return Ok(sections);
        }
    }
}
