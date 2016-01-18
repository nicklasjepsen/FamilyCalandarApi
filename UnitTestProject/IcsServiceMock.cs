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
#pragma warning disable 1998
        public async Task<IcsCalendarModel> GetIcsContent(string path, string etag)
#pragma warning restore 1998
        {
            return new IcsCalendarModel {Appointments = IcsParser.ParseAppointments(File.ReadAllLines("US-Holidays.ics"))};
        }
    }
}
