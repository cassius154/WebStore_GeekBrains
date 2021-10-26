using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebStore.Controllers;
using WebStore.Domain.Entities.Orders;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;
using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers
{
    [TestClass]
    public class CartControllerTests
    {
        [TestMethod]
        public async Task CheckOutModelStateInvalidReturnsViewWithModel()
        {
            const string expectedDescription = "Test description";

            var cartServiceMock = new Mock<ICartService>();

            var controller = new CartController(cartServiceMock.Object);
            controller.ModelState.AddModelError("error", "Invalid model");

            var orderModel = new OrderViewModel
            {
                Description = expectedDescription,
            };
            var orderServiceMock = new Mock<IOrderService>();

            var result = await controller.CheckOut(orderModel, orderServiceMock.Object);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CartOrderViewModel>(viewResult.Model);

            Assert.Equal(expectedDescription, model.Order.Description);

            cartServiceMock.Verify(s => s.GetViewModel());
            cartServiceMock.VerifyNoOtherCalls();
            orderServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task CheckOutModelStateValidCallServiceAndReturnsRedirect()
        {
            const string expectedUser = "Test user";

            const string expectedDescription = "Test description";
            const string expectedAddress = "Test address";
            const string expectedPhone = "Test phone";

            var cartServiceMock = new Mock<ICartService>();
            cartServiceMock
               .Setup(c => c.GetViewModel())
               .Returns(new CartViewModel
               {
                   Items = new[] { (new ProductViewModel { Name = "Test product" }, 1) }
               });


            const int expectedOrderId = 1;
            var orderServiceMock = new Mock<IOrderService>();
            orderServiceMock
               .Setup(c => c.CreateOrder(It.IsAny<string>(), It.IsAny<CartViewModel>(), It.IsAny<OrderViewModel>()))
               .ReturnsAsync(new Order
               {
                   Id = expectedOrderId,
                   Description = expectedDescription,
                   Address = expectedAddress,
                   Phone = expectedPhone,
                   Date = DateTime.Now,
                   Items = Array.Empty<OrderItem>(),
               });

            var controller = new CartController(cartServiceMock.Object)
            {
                ControllerContext = new()
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, expectedUser) }))
                    }
                }
            };

            var orderModel = new OrderViewModel
            {
                Address = expectedAddress,
                Phone = expectedPhone,
                Description = expectedDescription,
            };
            

            var result = await controller.CheckOut(orderModel, orderServiceMock.Object);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(CartController.OrderConfirmed), redirectResult.ActionName);
            Assert.Null(redirectResult.ControllerName);

            Assert.Equal(expectedOrderId, redirectResult.RouteValues["id"]);
        }
    }
}
