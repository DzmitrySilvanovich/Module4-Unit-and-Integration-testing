using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketing.DAL.Domain
{
    public class Section
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int VenueId { get; set; }
        public int PriceTypeId { get; set; }
        public List<Seat> Seats { get; set; } = new List<Seat>();
    }
}
