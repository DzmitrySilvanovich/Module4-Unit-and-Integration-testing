using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.DAL.Domain
{
    public class PaymentStatus
    {
        public PaymentState Id { get; set; }
        public required string Name { get; set; }
    }
}
