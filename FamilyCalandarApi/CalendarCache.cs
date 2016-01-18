using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemOut.CalandarApi
{
    public interface ICalendarCache
    {
        CalendarCacheEntry GetCalendar(string id);
        CalendarCacheEntry PutCalendar(string id, CalendarCacheEntry calendarModel);
    }

    internal class CalendarCache : ICalendarCache
    {
        private readonly ConcurrentDictionary<string, CalendarCacheEntry> cache;

        public CalendarCache()
        {
            cache = new ConcurrentDictionary<string, CalendarCacheEntry>();
        }

        public CalendarCacheEntry GetCalendar(string id)
        {
            CalendarCacheEntry cachedEntry;
            if (cache.TryGetValue(id, out cachedEntry))
            {
                return cachedEntry;
            }
            return null;
        }

        public CalendarCacheEntry PutCalendar(string id, CalendarCacheEntry calendarCacheModel)
        {
            return cache.AddOrUpdate(id, calendarCacheModel, (id1, entry) => calendarCacheModel);
        }
    }

    public class CalendarCacheEntry
    {
        public string Id { get; set; }
        public string ETag { get; set; }
        public CalendarModel CalendarModel { get; set; }

        public CalendarCacheEntry(string id)
        {
            Id = id;
            CalendarModel = new CalendarModel();
        }
    }
}
