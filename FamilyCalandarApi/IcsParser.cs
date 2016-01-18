using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemOut.Toolbox.Core;

namespace SystemOut.CalandarApi
{
    public static class IcsParser
    {
        public static IEnumerable<AppointmentModel> ParseAppointments(IReadOnlyList<string> icsLines)
        {
            var events = new ConcurrentBag<AppointmentModel>();
            Parallel.For(0, icsLines.Count, x =>
            {
                if (icsLines[x] == "BEGIN:VEVENT")
                {
                    var y = x + 1;
                    var props = new Dictionary<string, string>();
                    do
                    {
                        var line = icsLines[y];
                        y++;
                        if (!line.Contains(":"))
                            continue;
                        var splitterIndex = line.IndexOf(':');
                        var key = line.Substring(0, splitterIndex);
                        var value = line.Substring(splitterIndex + 1, line.Length - splitterIndex - 1);
                        if (props.ContainsKey(key))
                            continue;
                        if (key.Contains("DTSTART;TZID=") ||
                            key.Contains("DTEND;TZID="))
                        {
                            var result = ParseDateForSpecificTimezone(key, value);
                            props.Add(result.Key, result.Value);
                        }
                        else
                        {
                            props.Add(key, value);
                        }
                    } while (icsLines[y] != "END:VEVENT");
                    var vevent = new AppointmentModel();
#pragma warning disable 168
                    string created, summary, startTime, endTime, sequence, uid;
#pragma warning restore 168
                    //if (props.TryGetValue("CREATED", out created))
                    //{
                    //    // 20141110T180231Z
                    //    vevent. = DateTime.ParseExact(created, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);
                    //}
                    if (props.TryGetValue("SUMMARY", out summary))
                    {
                        vevent.Subject = summary;
                    }
                    // TODO: Handle timezone
                    if (props.TryGetValue("DTSTART_PARSED", out startTime))
                    {
                        vevent.StartTime = DateTime.ParseExact(startTime, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture);
                    }
                    else if (props.TryGetValue("DTSTART;VALUE=DATE", out startTime))
                    {
                        vevent.StartTime = DateTime.ParseExact(startTime, "yyyyMMdd", CultureInfo.InvariantCulture);
                    }
                    if (props.TryGetValue("DTEND_PARSED", out endTime))
                    {
                        vevent.EndTime = DateTime.ParseExact(endTime, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture);
                    }
                    else if (props.TryGetValue("DTEND;VALUE=DATE", out endTime))
                    {
                        vevent.EndTime = DateTime.ParseExact(endTime, "yyyyMMdd", CultureInfo.InvariantCulture);
                    }
                    //if (props.TryGetValue("SEQUENCE", out sequence))
                    //{
                    //    vevent.Sequence = int.Parse(sequence);
                    //}
                    //if (props.TryGetValue("UID", out uid))
                    //{
                    //    vevent.Uid = uid;
                    //}
                    events.Add(vevent);
                }
            });

            return events;
        }

        private static DateTimeParseResult ParseDateForSpecificTimezone(string key, string value)
        {
            // Handle timezone different
            // First get timezone
            var splitted = key.Split('=');
            if (splitted.Length > 1)
            {
                var tzStr = splitted[1];
                var tz = Converters.OlsonTimeZoneToTimeZoneInfo(tzStr);
                var utcDt = TimeZoneInfo.ConvertTime(
                    DateTime.ParseExact(value, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture),
                    tz, TimeZoneInfo.Utc);

                return new DateTimeParseResult(key.Split(';').First() + "_PARSED", utcDt.ToString("yyyyMMddTHHmmss"));

            }
            return new DateTimeParseResult(key, value);
        }

        class DateTimeParseResult
        {
            public string Key { get; private set; }
            public string Value { get; private set; }

            public DateTimeParseResult(string key, string value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
