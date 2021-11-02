using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebStore.Controllers;
using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;

using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers
{
    [TestClass]
    public class CatalogControllerTests
    {
        [TestMethod]
        public void DetailsReturnsWithCorrectView()
        {
            const int expectedId = 321;
            const string expectedName = "Test product";
            const int expectedOrder = 5;
            const decimal expectedPrice = 13.5m;
            const string expectedImgUrl = "/img/product.img";

            const int expectedBrandId = 7;
            const string expectedBrandName = "Test brand";
            const int expectedBrandOrder = 10;

            const int expectedSectionId = 14;
            const string expectedSectionName = "Test section";
            const int expectedSectionOrder = 123;

            var productServiceMock = new Mock<IProductService>();
            productServiceMock
               //.Setup(s => s.GetProductById(It.IsAny<int>()))
               .Setup(s => s.GetProductById(It.Is<int>(id => id > 0)))
               .Returns<int>(id => new Product
               {
                   Id = id,
                   Name = expectedName,
                   Order = expectedOrder,
                   Price = expectedPrice,
                   ImageUrl = expectedImgUrl,
                   BrandId = expectedBrandId,
                   Brand = new()
                   {
                       Id = expectedBrandId,
                       Name = expectedBrandName,
                       Order = expectedBrandOrder,
                   },
                   SectionId = expectedSectionId,
                   Section = new()
                   {
                       Id = expectedSectionId,
                       Name = expectedSectionName,
                       Order = expectedSectionOrder,
                   }
               });

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["CatalogPageSize"]).Returns("4");

            var loggerMock = new Mock<ILogger<CatalogController>>();
            
            var controller = new CatalogController(productServiceMock.Object, configurationMock.Object, loggerMock.Object);

            var result = controller.Details(expectedId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<ProductViewModel>(viewResult.Model);

            Assert.Equal(expectedId, model.Id);
            Assert.Equal(expectedName, model.Name);
            Assert.Equal(expectedPrice, model.Price);
            Assert.Equal(expectedImgUrl, model.ImageUrl);
            Assert.Equal(expectedBrandName, model.Brand);
            Assert.Equal(expectedSectionName, model.Section);

            productServiceMock.Verify(s => s.GetProductById(It.Is<int>(id => id > 0)));
            productServiceMock.VerifyNoOtherCalls();
        }
    }
}
