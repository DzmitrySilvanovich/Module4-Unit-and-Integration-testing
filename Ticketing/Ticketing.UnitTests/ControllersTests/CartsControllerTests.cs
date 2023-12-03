using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Model;
using Ticketing.DAL.Domain;
using Ticketing.UI.Controllers;

namespace Ticketing.UnitTests.ControllersTests
{
    public class CartsControllerTests
    {
        private readonly IFixture _fixture;

        public CartsControllerTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetAsync_Succes()
        {
            var collection = _fixture.Build<ShoppingCartReturnModel>()
             .CreateMany(1)
             .ToList();

            Mock<ICartService> service = new Mock<ICartService>();

            service.Setup(s => s.CartItemsAsync(It.IsAny<Guid>())).ReturnsAsync(collection);

            var controller = new CartsController(service.Object);
            var result = await controller.GetAsync(Guid.NewGuid());

            service.Verify(u => u.CartItemsAsync(It.IsAny<Guid>()), Times.Once, "fail");
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetAsync_fail()
        {
            var collection = _fixture.Build<ShoppingCartReturnModel>()
             .CreateMany(0)
             .ToList();

            Mock<ICartService> service = new Mock<ICartService>();

            service.Setup(s => s.CartItemsAsync(It.IsAny<Guid>())).ReturnsAsync(collection);

            var controller = new CartsController(service.Object);
            var result = await controller.GetAsync(Guid.NewGuid());

            service.Verify(u => u.CartItemsAsync(It.IsAny<Guid>()), Times.Once, "fail");
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Post_Succesc()
        {
            var model = _fixture.Build<CartStateReturnModel>().Create();

            Mock<ICartService> service = new Mock<ICartService>();

            service.Setup(s => s.AddSeatToCartAsync(It.IsAny<Guid>(), It.IsAny<OrderCartModel>())).Returns(Task.FromResult(model));

            var controller = new CartsController(service.Object);
            var result = await controller.Post(Guid.NewGuid(), _fixture.Build<OrderCartModel>().Create());

            service.Verify(u => u.AddSeatToCartAsync(It.IsAny<Guid>(), It.IsAny<OrderCartModel>()), Times.Once, "fail");
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PutAsync_Succesc()
        {
            var model = _fixture.Create<int>();

            Mock<ICartService> service = new Mock<ICartService>();

            service.Setup(s => s.BookSeatToCartAsync(It.IsAny<Guid>())).Returns(Task.FromResult(model));

            var controller = new CartsController(service.Object);
            var result = await controller.PutAsync(Guid.NewGuid());

            service.Verify(u => u.BookSeatToCartAsync(It.IsAny<Guid>()), Times.Once, "fail");
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PutAsync_Fail()
        {
            Mock<ICartService> service = new Mock<ICartService>();

            service.Setup(s => s.BookSeatToCartAsync(It.IsAny<Guid>())).Returns(Task.FromResult(0));

            var controller = new CartsController(service.Object);
            var result = await controller.PutAsync(Guid.NewGuid());

            service.Verify(u => u.BookSeatToCartAsync(It.IsAny<Guid>()), Times.Once, "fail");
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_Success()
        {
            Mock<ICartService> service = new Mock<ICartService>();
            service.Setup(s => s.DeleteSeatForCartAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(true));
            var controller = new CartsController(service.Object);

            var result = await controller.DeleteAsync(Guid.NewGuid(), 1, 1);

            service.Verify(u => u.DeleteSeatForCartAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once, "fail");
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_Fail()
        {
            Mock<ICartService> service = new Mock<ICartService>();
            service.Setup(s => s.DeleteSeatForCartAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(false));
            var controller = new CartsController(service.Object);

            var result = await controller.DeleteAsync(Guid.NewGuid(), 1, 1);

            service.Verify(u => u.DeleteSeatForCartAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once, "fail");
            Assert.IsType<BadRequestObjectResult > (result);
        }

    }
}
