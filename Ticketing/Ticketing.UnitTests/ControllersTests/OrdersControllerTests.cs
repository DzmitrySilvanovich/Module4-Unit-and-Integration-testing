using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.BAL.Contracts;
using Ticketing.UI.Controllers;

namespace Ticketing.UnitTests.ControllersTests
{
    public class OrdersControllerTests
    {
        private readonly IFixture _fixture;

        public OrdersControllerTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task DeleteAsync_Success()
        {
            Mock<IOrderService> service = new Mock<IOrderService>();

            service.Setup(s => s.ReleaseCartsFromOrderAsync(It.IsAny<int>())).Returns(Task.FromResult(true));

            var controller = new OrdersController(service.Object);
            var result = await controller.DeleteAsync(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteFailedAsync_Fail()
        {
            Mock<IOrderService> service = new Mock<IOrderService>();

            service.Setup(s => s.ReleaseCartsFromOrderAsync(It.IsAny<int>())).Returns(Task.FromResult(false));

            var controller = new OrdersController(service.Object);
            var result = await controller.DeleteAsync(1);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
