using System;

// ReSharper disable once CheckNamespace
namespace SystemOut.CalandarApi
{
    public class AppointmentModel
    {
        public string Subject { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsPrivate { get; set; }
    }
}
