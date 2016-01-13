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
        public string[] GetIcsContent(string path)
        {
            return File.ReadAllLines("US-Holidays.ics");
        }
    }
}
