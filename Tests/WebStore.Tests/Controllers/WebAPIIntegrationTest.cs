using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;
using WebStore.Interfaces.TestAPI;
using WebStore.Services.DTO;
using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers
{
    [TestClass]
    public class WebAPIIntegrationTest
    {
        private WebApplicationFactory<Startup> _host;

        private readonly string[] _expectedValues = Enumerable.Range(1, 10).Select(i => $"TestValue - {i}").ToArray();

        private readonly Product[] _products = new[]
            {
                new Product
                {
                    Id = 1,
                    Name = "Product 1",
                    Price = 1.1m,
                    Order = 1,
                    ImageUrl = "img_1.png",
                    Brand = new Brand { Id = 1, Name = "Brand 1", Order = 1 },
                    SectionId = 1,
                    Section = new Section { Id = 1, Name = "Section 1", Order = 1 },
                },
                new Product
                {
                    Id = 2,
                    Name = "Product 2",
                    Price = 2.2m,
                    Order = 2,
                    ImageUrl = "img_2.png",
                    Brand = new Brand { Id = 2, Name = "Brand 2", Order = 2 },
                    SectionId = 2,
                    Section = new Section { Id = 2, Name = "Section 2", Order = 2 },
                },
                new Product
                {
                    Id = 3,
                    Name = "Product 3",
                    Price = 3.3m,
                    Order = 3,
                    ImageUrl = "img_3.png",
                    Brand = new Brand { Id = 3, Name = "Brand 3", Order = 3 },
                    SectionId = 3,
                    Section = new Section { Id = 3, Name = "Section 3", Order = 3 },
                },
            };


        [TestInitialize]
        public void Initialize()
        {
            var valuesServiceMock = new Mock<IValuesClient>();
            valuesServiceMock.Setup(s => s.GetAll()).Returns(_expectedValues);

            var cartServiceMock = new Mock<ICartService>();
            cartServiceMock.Setup(s => s.GetViewModel()).Returns(
                new CartViewModel
                {
                    Items = _products.Select(p => (p.ToProductView(), 1)) 
                });

            //здесь мы по сути подменяем конфигурирование в
            //public static IHostBuilder CreateHostBuilder(string[] args) класса Program
            _host = new WebApplicationFactory<Startup>()
               .WithWebHostBuilder(host => host
                   .ConfigureServices(services => services
                       .AddSingleton(valuesServiceMock.Object)  //внедряем псевдоклиента в главный WebStore
                       .AddSingleton(cartServiceMock.Object)));  
                       
        }

        [TestMethod]
        public async Task GetValues()
        {
            var _exceptedCartContent = $"Корзина ({_products.Length})";

            var client = _host.CreateClient();

            var response = await client.GetAsync("/WebAPI");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var parser = new HtmlParser();

            var contentStream = await response.Content.ReadAsStreamAsync();
            var html = parser.ParseDocument(contentStream);

            var items = html.QuerySelectorAll(".container table.table tbody tr td:last-child");

            var actualValues = items.Select(item => item.Text());

            Assert.Equal(_expectedValues, actualValues);

            var cart = html.QuerySelectorAll("div.shop-menu.pull-right ul.nav.navbar-nav li a#cart-container");
            var actualCartContent = cart?.FirstOrDefault()?.Text()?.Trim();

            Assert.Equal(_exceptedCartContent, actualCartContent);
        }

    }
}
