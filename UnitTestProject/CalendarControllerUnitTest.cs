using System;
using SystemOut.CalandarApi;
using SystemOut.CalandarApi.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class CalendarControllerUnitTest
    {
        [TestMethod]
        public void GetCalendarTest()
        {
            var controller = new CalendarController(new CredentialProviderMock {ValidId = "id"}, new IcsServiceMock());
            var events = controller.Get("id");
            Assert.IsNotNull(events);
        }
    }
}
