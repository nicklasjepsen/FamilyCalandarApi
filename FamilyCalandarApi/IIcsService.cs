using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemOut.CalandarApi
{
    public interface IIcsService
    {
        Task<IcsCalendarModel> GetIcsContent(string path, string etag);
    }

    public class IcsCalendarModel
    {
        public IEnumerable<AppointmentModel> Appointments { get; set; }
        public string ETag { get; set; }
        public bool NotModified { get; set; }
    }
}
