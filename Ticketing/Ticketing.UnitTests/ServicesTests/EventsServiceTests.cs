using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.BAL.Services;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;
using Ticketing.UnitTests.Helpers;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.UnitTests.ServicesTests
{
    public class EventsServiceTests
    {
        [Fact]
        public async Task GetEventsAsync_Success()
        {
            var service = PrepareDataForSuccess();

            var eventCollection = await service.GetEventsAsync();

            var eventArray = eventCollection.ToArray();

            Assert.Equal(3, eventArray.Length);
            Assert.Collection(eventArray,
               item => Assert.Equal("Event1", item.Name),
               item => Assert.Equal("Event2", item.Name),
               item => Assert.Equal("Event3", item.Name));
        }

        [Fact]
        public async Task GetEventsAsync_Fail()
        {
            var service = PrepareDataForFail();

            var eventCollection = await service.GetEventsAsync();

            Assert.Empty(eventCollection);
        }

        [Fact]
        public async Task GetSeatsAsync_Success()
        {
            var service = PrepareDataForSuccess();
            var result = await service.GetSeatsAsync(1, 1);

            Assert.Single(result);
            Assert.Collection(result,
                item => { Assert.Equal(1, item.SeatId);
                    Assert.Equal(1, item.RowNumber);
                    Assert.Equal(1, item.SectionId);
                    Assert.Equal("Available", item.NameSeatStatus);
                    Assert.Equal("Adult", item.NamePriceType);
                }
            );
        }

        [Fact]
        public async Task GetSeatsAsync_Fail()
        {
            var service = PrepareDataForFail();
            var result = await service.GetSeatsAsync(1, 1);

            Assert.Empty(result);
        }

        public static EventService PrepareDataForSuccess()
        {
            var events = DataHelper.EventsInitialization();
            var seatStatuses = DataHelper.SeatStatusesInitialization();
            var priceType = DataHelper.PriceTypesInitialization();
            var shoppingCarts = DataHelper.ShoppingCartsInitialization();
            var seats = DataHelper.SeatsInitialization();

            var mockShoppingCartSet = MockDbSet.BuildAsync(shoppingCarts);
            var mockSeatSet = MockDbSet.BuildAsync(seats);
            var mockEventSet = MockDbSet.BuildAsync(events);
            var mockSeatStatusSet = MockDbSet.BuildAsync(seatStatuses);
            var mockPriceTypeSet = MockDbSet.BuildAsync(priceType);

            var mockContext = new Mock<ApplicationContext>();
            mockContext.Setup<DbSet<ShoppingCart>>(c => c.ShoppingCarts).Returns(mockShoppingCartSet.Object);
            mockContext.Setup<DbSet<Seat>>(c => c.Seats).Returns(mockSeatSet.Object);
            mockContext.Setup<DbSet<Event>>(c => c.Events).Returns(mockEventSet.Object);
            mockContext.Setup<DbSet<SeatStatus>>(c => c.SeatStatuses).Returns(mockSeatStatusSet.Object);
            mockContext.Setup<DbSet<PriceType>>(c => c.PriceTypes).Returns(mockPriceTypeSet.Object);

            Mock<Repository<Event>> mockEventRepository = new Mock<Repository<Event>>(mockContext.Object);
            Mock<Repository<ShoppingCart>> mockShoppingCartsRepository = new Mock<Repository<ShoppingCart>>(mockContext.Object);
            Mock<Repository<Seat>> mockSeatRepository = new Mock<Repository<Seat>>(mockContext.Object);
            Mock<Repository<PriceType>> mockPriceTypeRepository = new Mock<Repository<PriceType>>(mockContext.Object);
            Mock<Repository<SeatStatus>> mockSeatStatusRepository = new Mock<Repository<SeatStatus>>(mockContext.Object);

            mockShoppingCartsRepository.Setup(c => c.GetAllAsync()).ReturnsAsync(mockShoppingCartSet.Object);
            mockEventRepository.Setup(c => c.GetAllAsync()).ReturnsAsync(mockEventSet.Object);
            mockSeatRepository.Setup(c => c.GetAllAsync()).ReturnsAsync(mockSeatSet.Object);
            mockPriceTypeRepository.Setup(c => c.GetAllAsync()).ReturnsAsync(mockPriceTypeSet.Object);
            mockSeatStatusRepository.Setup(c => c.GetAllAsync()).ReturnsAsync(mockSeatStatusSet.Object);

            var service = new EventService(mockEventRepository.Object,
                mockSeatRepository.Object,
                mockSeatStatusRepository.Object,
                mockPriceTypeRepository.Object,
                mockShoppingCartsRepository.Object);

            return service;
        }

        public static EventService PrepareDataForFail()
        {
            var events = DataHelper.EventsInitialization();
            var seatStatuses = DataHelper.SeatStatusesInitialization();
            var priceType = DataHelper.PriceTypesInitialization();
            var shoppingCarts = DataHelper.ShoppingCartsInitialization();
            var seats = DataHelper.SeatsInitialization();


            var mockShoppingCartSet = MockDbSet.BuildAsync(shoppingCarts);
            var mockSeatSet = MockDbSet.BuildAsync(seats);
            var mockEventSet = MockDbSet.BuildAsync(events);
            var mockSeatStatusSet = MockDbSet.BuildAsync(seatStatuses);
            var mockPriceTypeSet = MockDbSet.BuildAsync(priceType);

            var mockContext = new Mock<ApplicationContext>();
            mockContext.Setup<DbSet<ShoppingCart>>(c => c.ShoppingCarts).Returns(mockShoppingCartSet.Object);
            mockContext.Setup<DbSet<Seat>>(c => c.Seats).Returns(mockSeatSet.Object);
            mockContext.Setup<DbSet<Event>>(c => c.Events).Returns(mockEventSet.Object);
            mockContext.Setup<DbSet<SeatStatus>>(c => c.SeatStatuses).Returns(mockSeatStatusSet.Object);
            mockContext.Setup<DbSet<PriceType>>(c => c.PriceTypes).Returns(mockPriceTypeSet.Object);

            Mock<Repository<Event>> mockEventRepository = new Mock<Repository<Event>>(mockContext.Object);
            Mock<Repository<ShoppingCart>> mockShoppingCartsRepository = new Mock<Repository<ShoppingCart>>(mockContext.Object);
            Mock<Repository<Seat>> mockSeatRepository = new Mock<Repository<Seat>>(mockContext.Object);
            Mock<Repository<PriceType>> mockPriceTypeRepository = new Mock<Repository<PriceType>>(mockContext.Object);
            Mock<Repository<SeatStatus>> mockSeatStatusRepository = new Mock<Repository<SeatStatus>>(mockContext.Object);

            mockShoppingCartsRepository.Setup(c => c.GetAllAsync()).ReturnsAsync(mockShoppingCartSet.Object);
            mockPriceTypeRepository.Setup(c => c.GetAllAsync()).ReturnsAsync(mockPriceTypeSet.Object);
            mockSeatStatusRepository.Setup(c => c.GetAllAsync()).ReturnsAsync(mockSeatStatusSet.Object);

            var service = new EventService(mockEventRepository.Object,
                mockSeatRepository.Object,
                mockSeatStatusRepository.Object,
                mockPriceTypeRepository.Object,
                mockShoppingCartsRepository.Object);

            return service;
        }
    }
}
