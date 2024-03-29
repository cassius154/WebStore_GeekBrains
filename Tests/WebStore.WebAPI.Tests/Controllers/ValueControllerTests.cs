﻿using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebStore.Interfaces;

namespace WebStore.WebAPI.Tests.Controllers
{
    [TestClass]
    public class ValueControllerTests
    {
        private readonly WebApplicationFactory<Startup> _host = new();


        [TestMethod]
        public async Task GetValuesIntegrityTest()
        {
            //создаем httpClient, который отправляет виртуальные запросы
            //но не в сеть, а к _host (эмулирует)
            var client = _host.CreateClient();

            var response = await client.GetAsync(WebAPIAddresses.Values);

            response.EnsureSuccessStatusCode();

            var values = await response.Content.ReadFromJsonAsync<IEnumerable<string>>();
            //далее можно проверять/тестировать значения

            //если контроллер возвращает html - можно анализировать
            //с помощью, например, AngleSharp
            //var parser = new HtmlParser();
            //var html = parser.ParseDocument(await response.Content.ReadAsStreamAsync());
        }
    }
}
