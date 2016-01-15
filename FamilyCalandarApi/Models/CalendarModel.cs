using System;
using System.Collections.Generic;
// ReSharper disable once CheckNamespace
namespace SystemOut.CalandarApi
{
    public class CalendarModel
    {
        public DateTime LastChangeDate { get; set; }
        public string Owner { get; set; }

        public IEnumerable<AppointmentModel> Appointments { get; set; }
    }
}
