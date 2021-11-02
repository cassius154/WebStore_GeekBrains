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
using WebStore.Interfaces.TestAPI;
using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers
{
    [TestClass]
    public class WebAPIIntegrationTest
    {
        private WebApplicationFactory<Startup> _host;

        private readonly string[] _expectedValues = Enumerable.Range(1, 10).Select(i => $"TestValue - {i}").ToArray();

        [TestInitialize]
        public void Initialize()
        {
            var valuesServiceMock = new Mock<IValuesClient>();
            valuesServiceMock.Setup(s => s.GetAll()).Returns(_expectedValues);

            //здесь мы по сути подменяем конфигурирование в
            //public static IHostBuilder CreateHostBuilder(string[] args) класса Program
            _host = new WebApplicationFactory<Startup>()
               .WithWebHostBuilder(host => host
                   .ConfigureServices(services => services
                       .AddSingleton(valuesServiceMock.Object)));  //внедряем псевдоклиента в главный WebStore
        }

        [TestMethod]
        public async Task GetValues()
        {
            var client = _host.CreateClient();

            var response = await client.GetAsync("/WebAPI");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var parser = new HtmlParser();

            var contentStream = await response.Content.ReadAsStreamAsync();
            var html = parser.ParseDocument(contentStream);

            var items = html.QuerySelectorAll(".container table.table tbody tr td:last-child");

            var actualValues = items.Select(item => item.Text());

            Assert.Equal(_expectedValues, actualValues);
        }

    }
}
