using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.BAL.Model;
using Ticketing.BAL.Services;
using Ticketing.DAL.Contracts;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;
using Ticketing.UnitTests.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.UnitTests.ServicesTests
{
    public class PaymentServiceTests
    {
        static List<Payment> payments = new List<Payment>();
        static List<PaymentStatus> paymentStatuses = new List<PaymentStatus>();
        static List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
        static List<Seat> seats = new List<Seat>();

        static Mock<Repository<Payment>>? _mockPaymentRepository = null;
        static Mock<Repository<PaymentStatus>>? _mockPaymentStatusRepository = null;
        static Mock<Repository<ShoppingCart>>? _mockShoppingCartRepository = null;
        static Mock<Repository<Seat>>? _mockSeatRepository = null;

        [Theory]
        [InlineData(1)]
        public async Task GetPaymentStatusAsync_Success(int Id)
        {
            var service = PrepareDataForSuccess();

            var expected = payments.Join(
                paymentStatuses,
                payment => payment.PaymentStatusId,
                state => state.Id,
                (payment, state) => new PaymentStatusReturnModel { Id = (int)state.Id, Name = state.Name }
            ).First();

            var paymentState = paymentStatuses.First().Id;

            _mockPaymentRepository.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int i) => payments.ToArray().Single(p => p.Id == i));
            _mockPaymentStatusRepository.Setup(c => c.GetByIdAsync(It.IsAny<PaymentState>()))
                .ReturnsAsync((PaymentState pmntState) => paymentStatuses.ToArray().Single(p => p.Id == pmntState));

            var result = await service.GetPaymentStatusAsync(Id);

            Assert.NotNull(result);
            Assert.Equal(expected.Id, result.Id);
            Assert.Equal(expected.Name, result.Name);
        }

        [Fact]
        public async Task CompletePaymentAsync_Success()
        {
            var service = PrepareDataForSuccess();
            var result = await service.CompletePaymentAsync(1);

            Assert.True(result);
            _mockPaymentRepository.Verify((p) => p.UpdateAsync(It.IsAny<Payment>()), Times.Once, ".Update payment is fail");
            _mockSeatRepository.Verify((p) => p.UpdateAsync(It.IsAny<Seat>()), Times.Once, ".Update seat is fail");
        }

        [Fact]
        public async Task FailPaymentAsync_Success()
        {
            var service = PrepareDataForSuccess();
            var result = await service.FailPaymentAsync(1);

            Assert.True(result);
            _mockPaymentRepository.Verify((p) => p.UpdateAsync(It.IsAny<Payment>()), Times.Once, ".Update payment is fail");
            _mockSeatRepository.Verify((p) => p.UpdateAsync(It.IsAny<Seat>()), Times.Once, ".Update seat is fail");
        }

        [Theory]
        [InlineData(10)]
        public async Task GetPaymentStatusAsync_Fail(int Id)
        {
            var service = PrepareDataForSuccess();

            var expected = payments.Join(
                paymentStatuses,
                payment => payment.PaymentStatusId,
                state => state.Id,
                (payment, state) => new PaymentStatusReturnModel { Id = (int)state.Id, Name = state.Name }
            ).First();

            var paymentState = paymentStatuses.First().Id;

            _mockPaymentRepository.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int i) => payments.FirstOrDefault(p => p.Id == i));
            _mockPaymentStatusRepository.Setup(c => c.GetByIdAsync(It.IsAny<PaymentState>()))
                .ReturnsAsync((PaymentState pmntState) => paymentStatuses.FirstOrDefault(p => p.Id == pmntState));

            var result = await service.GetPaymentStatusAsync(Id);

            Assert.NotNull(result);
            Assert.Equal(0, result.Id);
            Assert.Equal("", result.Name);
        }


        [Theory]
        [InlineData(10)]
        public async Task CompletePaymentAsync_Fail(int Id)
        {
            var service = PrepareDataForSuccess();
            var result = await service.CompletePaymentAsync(Id);

            Assert.False(result);
        }

        [Theory]
        [InlineData(10)]
        public async Task FailPaymentAsync_Fail(int Id)
        {
            var service = PrepareDataForSuccess();
            var result = await service.FailPaymentAsync(Id);

            Assert.False(result);
        }

        public static PaymentService PrepareDataForSuccess()
        {
            payments = DataHelper.PaymentsInitialization();
            paymentStatuses = DataHelper.PaymentStatusesInitialization();
            shoppingCarts = DataHelper.ShoppingCartsInitialization();
            seats = DataHelper.SeatsInitialization();

            var mockPaymentSet = MockDbSet.BuildAsync(payments);
            var mockPaymentStatusSet = MockDbSet.BuildAsync(paymentStatuses);
            var mockShoppingCartSet = MockDbSet.BuildAsync(shoppingCarts);
            var mockSeatSet = MockDbSet.BuildAsync(seats);

            mockPaymentSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                       .Returns((object[] r) => new ValueTask<Payment>(payments.FirstOrDefault(b => b.Id == (int)r[0])));
            mockPaymentSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((object[] r) => payments.FirstOrDefault(p => p.Id == (int)r[0]));

            var mockContext = new Mock<ApplicationContext>();
            mockContext.Setup<DbSet<Payment>>(c => c.Payments).Returns(mockPaymentSet.Object);

            mockContext.Setup<DbSet<PaymentStatus>>(c => c.PaymentStatuses).Returns(mockPaymentStatusSet.Object);
            mockContext.Setup<DbSet<ShoppingCart>>(c => c.ShoppingCarts).Returns(mockShoppingCartSet.Object);
            mockContext.Setup<DbSet<Seat>>(c => c.Seats).Returns(mockSeatSet.Object);

            _mockPaymentRepository = new Mock<Repository<Payment>>(mockContext.Object);
            _mockPaymentStatusRepository = new Mock<Repository<PaymentStatus>>(mockContext.Object);
            _mockShoppingCartRepository = new Mock<Repository<ShoppingCart>>(mockContext.Object);
            _mockSeatRepository = new Mock<Repository<Seat>>(mockContext.Object);

            _mockPaymentRepository.Setup(c => c.GetAllAsync()).ReturnsAsync(mockPaymentSet.Object);
            _mockPaymentStatusRepository.Setup(c => c.GetAllAsync()).ReturnsAsync(mockPaymentStatusSet.Object);
            _mockShoppingCartRepository.Setup(c => c.GetAllAsync()).ReturnsAsync(mockShoppingCartSet.Object);
            _mockSeatRepository.Setup(c => c.GetAllAsync()).ReturnsAsync(mockSeatSet.Object);

            _mockPaymentRepository.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int i) => payments.FirstOrDefault(p => p.Id == i));
            _mockPaymentStatusRepository.Setup(c => c.GetByIdAsync(It.IsAny<PaymentState>()))
                .ReturnsAsync((PaymentState pmntState) => paymentStatuses.FirstOrDefault(p => p.Id == pmntState));

            var service = new PaymentService(_mockPaymentRepository.Object, _mockPaymentStatusRepository.Object, _mockShoppingCartRepository.Object, _mockSeatRepository.Object); ;

            return service;
        }

        public static PaymentService PrepareDataForFail()
        {
            payments = new List<Payment>();
            paymentStatuses = new List<PaymentStatus>();
            shoppingCarts = new List<ShoppingCart>();
            seats = new List<Seat>();

            var mockPaymentSet = MockDbSet.BuildAsync(payments);
            var mockPaymentStatusSet = MockDbSet.BuildAsync(paymentStatuses);
            var mockShoppingCartSet = MockDbSet.BuildAsync(shoppingCarts);
            var mockSeatSet = MockDbSet.BuildAsync(seats);

            mockPaymentSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                       .Returns((object[] r) => new ValueTask<Payment>(payments.FirstOrDefault(b => b.Id == (int)r[0])));
            mockPaymentSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((object[] r) => payments.FirstOrDefault(p => p.Id == (int)r[0]));

            var mockContext = new Mock<ApplicationContext>();
            mockContext.Setup<DbSet<Payment>>(c => c.Payments).Returns(mockPaymentSet.Object);

            mockContext.Setup<DbSet<PaymentStatus>>(c => c.PaymentStatuses).Returns(mockPaymentStatusSet.Object);
            mockContext.Setup<DbSet<ShoppingCart>>(c => c.ShoppingCarts).Returns(mockShoppingCartSet.Object);
            mockContext.Setup<DbSet<Seat>>(c => c.Seats).Returns(mockSeatSet.Object);

            _mockPaymentRepository = new Mock<Repository<Payment>>(mockContext.Object);
            _mockPaymentStatusRepository = new Mock<Repository<PaymentStatus>>(mockContext.Object);
            _mockShoppingCartRepository = new Mock<Repository<ShoppingCart>>(mockContext.Object);
            _mockSeatRepository = new Mock<Repository<Seat>>(mockContext.Object);

            _mockPaymentRepository.Setup(c => c.GetAllAsync()).ReturnsAsync(mockPaymentSet.Object);
            _mockPaymentStatusRepository.Setup(c => c.GetAllAsync()).ReturnsAsync(mockPaymentStatusSet.Object);
            _mockShoppingCartRepository.Setup(c => c.GetAllAsync()).ReturnsAsync(mockShoppingCartSet.Object);
            _mockSeatRepository.Setup(c => c.GetAllAsync()).ReturnsAsync(mockSeatSet.Object);

            _mockPaymentRepository.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int i) => payments.Single(p => p.Id == i));
            _mockPaymentStatusRepository.Setup(c => c.GetByIdAsync(It.IsAny<PaymentState>()))
                .ReturnsAsync((PaymentState pmntState) => paymentStatuses.Single(p => p.Id == pmntState));

            var service = new PaymentService(_mockPaymentRepository.Object, _mockPaymentStatusRepository.Object, _mockShoppingCartRepository.Object, _mockSeatRepository.Object); ;

            return service;
        }
    }
}
