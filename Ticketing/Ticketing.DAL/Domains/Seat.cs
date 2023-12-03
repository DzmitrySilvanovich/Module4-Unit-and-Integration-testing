using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.DAL.Enums;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.DAL.Domain
{
    public class Seat
    {
        public int Id { get; set; }
        public int RowNumber { get; set; }
        public int SeatNumber { get; set; }
        public SeatState SeatStatusState { get; set; }
        public int SectionId { get; set; }
    }
}
