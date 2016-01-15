using System;
using System.Collections.Generic;
// ReSharper disable once CheckNamespace
namespace SystemOut.CalandarApi
{
    public class CalendarModel
    {
        public Guid Watermark { get; set; }
        public string Owner { get; set; }

        public IEnumerable<AppointmentModel> Appointments { get; set; }
    }
}
