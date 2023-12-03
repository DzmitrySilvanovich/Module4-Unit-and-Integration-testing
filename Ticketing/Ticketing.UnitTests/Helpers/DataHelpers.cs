using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.UnitTests.Helpers
{
    public static class DataHelper
    {
        public static List<Venue> VenuesInitialization()
        {
            return new List<Venue>
            {
                new Venue {Name = "Venue1", Id = 1},
                new Venue {Name = "Venue2", Id = 2},
                new Venue {Name = "Venue3", Id = 3}
            };
        }

        public static List<Order> OrdersInitialization()
        {
            return new List<Order>
            {
                new Order {Id = 1, Name = "Order1", CartId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4")}
            };
        }

        public static List<ShoppingCart> ShoppingCartsInitialization()
        {
            return new List<ShoppingCart>
            {
                new ShoppingCart {
                    Id = 1,
                    EventId = 1,
                    SeatId = 1,
                    PriceTypeId = 1,
                    Price = 1,
                    CartId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4")}
             };
        }

        public static List<Seat> SeatsInitialization()
        {
            return new List<Seat>
            {
                new Seat { Id = 1, SectionId = 1, RowNumber = 1, SeatNumber = 1, SeatStatusState = SeatState.Available },
                new Seat { Id = 2, SectionId = 1, RowNumber = 1, SeatNumber = 2, SeatStatusState = SeatState.Available },
                new Seat { Id = 3, SectionId = 1, RowNumber = 1, SeatNumber = 3, SeatStatusState = SeatState.Available }
            };
        }

        public static List<Event> EventsInitialization()
        {
            return new List<Event>
            {
                new Event {Id = 1, Name = "Event1", EventDate = DateTime.Now.AddDays(-3)},
                new Event {Id = 2, Name = "Event2", EventDate = DateTime.Now},
                new Event {Id = 3, Name = "Event3", EventDate = DateTime.Now.AddDays(+3)}
            };
        }

        public static List<SeatStatus> SeatStatusesInitialization()
        {
            return new List<SeatStatus>
            {
                new SeatStatus {Id = (SeatState) 1, Name = "Available"},
                new SeatStatus {Id = (SeatState) 2, Name = "Booked"},
                new SeatStatus {Id = (SeatState) 3, Name = "Sold"}
            };
        }

        public static List<PaymentStatus> PaymentStatusesInitialization()
        {
            return new List<PaymentStatus>
            {
                new PaymentStatus {Id = (PaymentState) 1, Name = "No payment"},
                new PaymentStatus {Id = (PaymentState) 2, Name = "Part payment"},
                new PaymentStatus {Id = (PaymentState) 3, Name = "Full payment"},
                new PaymentStatus {Id = (PaymentState) 4, Name = "Payment Failed"}
            };
        }

        public static List<PriceType> PriceTypesInitialization()
        {
            return new List<PriceType>
            {
                new PriceType { Id = 1, Name = "Adult" },
                new PriceType { Id = 2, Name = "Child" },
                new PriceType { Id = 3, Name = "VIP" },
                new PriceType { Id = 4, Name = "Admission" }
            };
        }

        public static List<Section> SectionsInitialization()
        {
            return new List<Section>
            {
                new Section { Id = 1, Name = "Section1", VenueId = 1, PriceTypeId = 1 },
                new Section { Id = 2, Name = "Section2", VenueId = 1, PriceTypeId = 2 },
                new Section { Id = 3, Name = "Section1", VenueId = 2, PriceTypeId = 3 }
            };
        }

        public static List<Payment> PaymentsInitialization()
        {
            return new List<Payment>()
            {
                new Payment
                {
                    Id = 1,
                    Amount = 201.3m,
                    PaymentStatusId = (PaymentState) 1,
                    CartId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4")
                }
            };
        }
    }
}
