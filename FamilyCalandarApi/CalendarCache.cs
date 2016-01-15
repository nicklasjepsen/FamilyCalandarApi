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
        CalendarCacheEntry GetCalendar(Guid watermark);
        CalendarCacheEntry PutCalendar(CalendarModel calendarModel);
    }

    internal class CalendarCache : ICalendarCache
    {
        private readonly ConcurrentDictionary<Guid, InternalCalendarCacheEntry> cache;

        public CalendarCache()
        {
            cache = new ConcurrentDictionary<Guid, InternalCalendarCacheEntry>();
        }

        public CalendarCacheEntry GetCalendar(Guid watermark)
        {
            InternalCalendarCacheEntry cachedEntry;
            if (cache.TryGetValue(watermark, out cachedEntry))
            {
                if (!cachedEntry.IsExpired)
                    return new CalendarCacheEntry(cachedEntry);
            }

            return null;
        }

        public CalendarCacheEntry PutCalendar(CalendarModel calendarModel)
        {
            var cacheEntry = new InternalCalendarCacheEntry
            {
                CalendarModel = calendarModel,
                //ExpirationTime = DateTime.UtcNow.AddMinutes(5),
                ExpirationTime = DateTime.UtcNow.AddSeconds(10),
            };
            var guid = Guid.NewGuid();
            var internalCacheEntry = cache.AddOrUpdate(guid, cacheEntry, (guid1, entry) => cacheEntry);

            return new CalendarCacheEntry(internalCacheEntry);
        }
    }

    public class CalendarCacheEntry : BaseCalendarCacheEntry
    {
        public Guid Watermark { get; set; }

        public CalendarCacheEntry(BaseCalendarCacheEntry internalCalendarCacheEntry)
        {
            CalendarModel = internalCalendarCacheEntry.CalendarModel;
            ExpirationTime = internalCalendarCacheEntry.ExpirationTime;
        }
    }
    class InternalCalendarCacheEntry : BaseCalendarCacheEntry
    {
    }

    public class BaseCalendarCacheEntry
    {
        public CalendarModel CalendarModel { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool IsExpired => ExpirationTime >= DateTime.UtcNow;
    }
}
