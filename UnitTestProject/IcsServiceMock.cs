using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemOut.CalandarApi;

namespace UnitTestProject
{
    internal class IcsServiceMock : IIcsService
    {
        public async Task<IcsCalendarModel> GetIcsContent(string path, string etag)
        {
            return new IcsCalendarModel {IcsLines = File.ReadAllLines("US-Holidays.ics")};
        }
    }
}
