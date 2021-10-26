using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebStore.Controllers;
using WebStore.Interfaces.TestAPI;
using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers
{
    [TestClass]
    public class WebAPIControllerTests
    {
        //для создания контроллера WebAPIController можно передать в конструктор
        //или свою тестовую реализацию IValuesClient... (см *)
        //class TestValuesClient : IValuesClient
        //{
        //    public IEnumerable<string> GetAll() { throw new System.NotImplementedException(); }
        //    public int Count() { throw new System.NotImplementedException(); }
        //    public string GetById(int Id) { throw new System.NotImplementedException(); }
        //    public void Add(string Value) { throw new System.NotImplementedException(); }
        //    public void Edit(int Id, string Value) { throw new System.NotImplementedException(); }
        //    public bool Delete(int Id) { throw new System.NotImplementedException(); }
        //}

        [TestMethod]
        public void IndexReturnsWithDataValues()
        {
            //var valuesClient = TestValuesClient();
            //*) или создать Mock-объект
            var valuesClientMock = new Mock<IValuesClient>();

            //поскольку метод Index вызывает метод GetAll клиента - настраиваем Mock-объект
            var data = Enumerable.Range(1, 10)
               .Select(i => $"Value - {i}")
               .ToArray();
            //Можно отлаживаться и вот так
            //вывод будет и в окне Output (Debug)
            //и в TestExplorer в нижнем окне
            //и в окошке CodeLens - зеленая галка у метода
            Debug.WriteLine("Вывод данных в процессе тестирования " + data.Length);

            valuesClientMock
               //указываем, что когда будет вызван метод GetAll()
               .Setup(c => c.GetAll())
               //надо вернуть тестовые данные
               .Returns(data);

            //и передаем Mock-объект в конструктор контроллера
            var controller = new WebAPIController(valuesClientMock.Object);

            var result = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<IEnumerable<string>>(viewResult.Model);

            var i = 0;
            foreach (var actualValue in model)
            {
                var expectedValue = data[i++];
                Assert.Equal(expectedValue, actualValue);
            }

            //до этого мы проверяли Mock-объект в режиме Stub - эмуляции сервисов
            //в режиме mock нужно этот объект опрашивать
            valuesClientMock.Verify(s => s.GetAll()); //проверяем, что вызывался метод GetAll()
            valuesClientMock.VerifyNoOtherCalls(); //проверяем, что ничего другого не вызывалось
        }
    }
}
