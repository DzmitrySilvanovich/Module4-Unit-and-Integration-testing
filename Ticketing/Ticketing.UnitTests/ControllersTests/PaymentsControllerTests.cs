using AutoFixture;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
    public class PaymentsControllerTests
    {
        private readonly IFixture _fixture;

        public PaymentsControllerTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetAsync_Succes()
        {
            var model = _fixture.Build<PaymentStatusReturnModel>()
             .Create();

            Mock<IPaymentService> service = new Mock<IPaymentService>();

            service.Setup(s => s.GetPaymentStatusAsync(It.IsAny<int>())).Returns(Task.FromResult(model));

            var controller = new PaymentsController(service.Object);
            var result = await controller.GetAsync(1);

            service.Verify(u => u.GetPaymentStatusAsync(It.IsAny<int>()), Times.Once, "GetPaymentStatusAsync Fail");

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetAsync_Fail()
        {
            PaymentStatusReturnModel? model = null;

            Mock<IPaymentService> service = new Mock<IPaymentService>();

            service.Setup(s => s.GetPaymentStatusAsync(It.IsAny<int>())).Returns(Task.FromResult(model));

            var controller = new PaymentsController(service.Object);
            var result = await controller.GetAsync(1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutCompleteAsync_Succesl()
        {
            Mock<IPaymentService> service = new Mock<IPaymentService>();

            service.Setup(s => s.CompletePaymentAsync(It.IsAny<int>())).Returns(Task.FromResult(true));

            var controller = new PaymentsController(service.Object);
            var result = await controller.PutCompleteAsync(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PutCompleteAsync_Fail()
        {
            Mock<IPaymentService> service = new Mock<IPaymentService>();

            service.Setup(s => s.CompletePaymentAsync(It.IsAny<int>())).Returns(Task.FromResult(false));

            var controller = new PaymentsController(service.Object);
            var result = await controller.PutCompleteAsync(1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutFailedAsync_Succesl()
        {
            Mock<IPaymentService> service = new Mock<IPaymentService>();

            service.Setup(s => s.FailPaymentAsync(It.IsAny<int>())).Returns(Task.FromResult(true));

            var controller = new PaymentsController(service.Object);
            var result = await controller.PutFailedAsync(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PutFailedAsync_Fail()
        {
            Mock<IPaymentService> service = new Mock<IPaymentService>();

            service.Setup(s => s.FailPaymentAsync(It.IsAny<int>())).Returns(Task.FromResult(false));

            var controller = new PaymentsController(service.Object);
            var result = await controller.PutFailedAsync(1);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
