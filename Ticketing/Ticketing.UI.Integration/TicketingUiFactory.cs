using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.UI.Integration
{
    public class TicketingUiFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<ApplicationContext>));

                services.Remove(dbContextDescriptor);

                var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbConnection));

                services.Remove(dbConnectionDescriptor);

                services.AddDbContext<ApplicationContext>(opts => opts.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TicketDB_Test;Integrated Security=True;"));

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                using (var appContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>())
                {
                    try
                    {
                        appContext.Database.EnsureDeleted();
                        appContext.Database.EnsureCreated();
                        appContext.EnsureSeedData();
                    }
                    catch (Exception)
                    {
                         throw;
                    }
                }
            });
        }
    }

    public static class Seed
    {
        public static void EnsureSeedData(this ApplicationContext context)
        {
            if (!context.Database.GetPendingMigrations().Any())
            {
                if (!context.Venues.Any())
                {
                    context.Venues.Add(new Venue { Name = "Venue1" });
                    context.Venues.Add(new Venue { Name = "Venue2" });
                    context.Venues.Add(new Venue { Name = "Venue3" });
                    context.SaveChanges();
                }

                if (!context.Events.Any())
                {
                    context.Events.Add(new Event { Name = "Event1", EventDate = DateTime.Now.AddDays(-3) });
                    context.Events.Add(new Event { Name = "Event2", EventDate = DateTime.Now });
                    context.Events.Add(new Event { Name = "Event3", EventDate = DateTime.Now.AddDays(+3) });
                    context.SaveChanges();
                }

                if (!context.Carts.Any())
                {
                    context.Carts.Add(new Cart { Id = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4") });
                    context.SaveChanges();
                }

                if (!context.Orders.Any())
                {
                    context.Orders.Add(new Order { Name = "Order1", CartId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4") });
                    context.SaveChanges();
                }

                if (!context.PriceTypes.Any())
                {
                    context.PriceTypes.Add(new PriceType { Name = "Adult" });
                    context.PriceTypes.Add(new PriceType { Name = "Child" });
                    context.PriceTypes.Add(new PriceType { Name = "VIP" });
                    context.PriceTypes.Add(new PriceType { Name = "Admission" });
                    context.SaveChanges();
                }

                if (!context.SeatStatuses.Any())
                {
                    context.SeatStatuses.Add(new SeatStatus { Id = SeatState.Available, Name = "Available" });
                    context.SeatStatuses.Add(new SeatStatus { Id = SeatState.Booked, Name = "Booked" });
                    context.SeatStatuses.Add(new SeatStatus { Id = SeatState.Sold, Name = "Sold" });
                    context.SaveChanges();
                }

                if (!context.PaymentStatuses.Any())
                {
                    context.PaymentStatuses.Add(new PaymentStatus { Id = PaymentState.NoPayment, Name = "No payment" });
                    context.PaymentStatuses.Add(new PaymentStatus { Id = PaymentState.PartPayment, Name = "Part payment" });
                    context.PaymentStatuses.Add(new PaymentStatus { Id = PaymentState.FullPayment, Name = "Full payment" });
                    context.PaymentStatuses.Add(new PaymentStatus { Id = PaymentState.PaymentFailed, Name = "Payment Failed" });
                    context.SaveChanges();
                }

                if (!context.Payments.Any())
                {
                    context.Payments.Add(new Payment { Amount = 0, PaymentStatusId = PaymentState.NoPayment, CartId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4") });
                    context.SaveChanges();
                }

                if (!context.Sections.Any())
                {
                    context.Sections.Add(new Section { Name = "Section1", VenueId = 1, PriceTypeId = 1 });
                    context.Sections.Add(new Section { Name = "Section2", VenueId = 1, PriceTypeId = 2 });
                    context.Sections.Add(new Section { Name = "Section3", VenueId = 1, PriceTypeId = 3 });
           
                    context.SaveChanges();
                }

                if (!context.Seats.Any())
                {
                    context.Seats.Add(new Seat { SectionId = 1, RowNumber = 1, SeatNumber = 1, SeatStatusState = SeatState.Available });
                    context.Seats.Add(new Seat { SectionId = 1, RowNumber = 1, SeatNumber = 2, SeatStatusState = SeatState.Available });
                    context.Seats.Add(new Seat { SectionId = 1, RowNumber = 1, SeatNumber = 3, SeatStatusState = SeatState.Available });
           
                    context.SaveChanges();
                }

                if (!context.ShoppingCarts.Any())
                {
                    context.ShoppingCarts.Add(new ShoppingCart
                    {
                        EventId = 1,
                        SeatId = 1,
                        PriceTypeId = 1,
                        Price = 1,
                        CartId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4")
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
