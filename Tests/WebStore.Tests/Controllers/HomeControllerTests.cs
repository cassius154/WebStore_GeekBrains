using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebStore.Controllers;

using Assert = Xunit.Assert;
using MsAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace WebStore.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public void IndexReturnsView()
        {
            var controller = new HomeController();
            
            var result = controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [TestMethod]
        public void ContactUsReturnsView()
        {
            var controller = new HomeController();

            var result = controller.ContactUs();

            Assert.IsType<ViewResult>(result);
        }

        [TestMethod]
        public void StatusWithId404ReturnsView()
        {
            // A-A-A = Arrange - Act - Assert

            #region Arrange

            const string id = "404";
            const string expectedViewName = "NotFoundPage";
            var controller = new HomeController();

            #endregion Arrange

            #region Act

            var result = controller.Status(id);

            #endregion Act

            #region Assert

            var viewResult = Assert.IsType<ViewResult>(result);

            var actualViewName = viewResult.ViewName;

            Assert.Equal(expectedViewName, actualViewName);

            #endregion Assert
        }

        [TestMethod]
        [DataRow("123")]
        [DataRow("QWE")]
        public void StatusWithIdReturnsView(string id)
        {
            var expectedContent = "Status code === " + id;
            var controller = new HomeController();

            var result = controller.Status(id);

            var contentResult = Assert.IsType<ContentResult>(result);

            var actualContent = contentResult.Content;

            Assert.Equal(expectedContent, actualContent);
            //Assert по сути выбрасывает AssertFailedException в случае ложности проверяемого утверждения
        }


        //3 разных способа проверить метод на выброс нужного Exception
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void StatusThrownArgumentNullExceptionWhenIdIsNull_1()
        {
            var controller = new HomeController();

            _ = controller.Status(null);
        }

        [TestMethod]
        public void StatusThrownArgumentNullExceptionWhenIdIsNull_2()
        {
            const string expectedParameterName = "id";
            var controller = new HomeController();

            Exception exception = null;
            try
            {
                _ = controller.Status(null);
            }
            catch (ArgumentNullException e)
            {
                exception = e;
            }

            if (exception is null)
            {
                //Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
                MsAssert.Fail();
            }

            var actualException = Assert.IsType<ArgumentNullException>(exception);
            var actualParameterName = actualException.ParamName;
            Assert.Equal(expectedParameterName, actualParameterName);
        }

        [TestMethod]
        public void StatusThrownArgumentNullExceptionWhenIdIsNull_3()
        {
            const string expectedParameterName = "id";
            var controller = new HomeController();

            var actualException = Assert.Throws<ArgumentNullException>(() => controller.Status(null));
            var actualParameterName = actualException.ParamName;
            Assert.Equal(expectedParameterName, actualParameterName);
        }
    }
}
