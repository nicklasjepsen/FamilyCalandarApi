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
        CalendarCacheEntry PutCalendar(string id, CalendarModel calendarModel);
    }

    internal class CalendarCache : ICalendarCache
    {
        private readonly ConcurrentDictionary<string, InternalCalendarCacheEntry> cache;

        public CalendarCache()
        {
            cache = new ConcurrentDictionary<string, InternalCalendarCacheEntry>();
        }

        public CalendarCacheEntry GetCalendar(string id)
        {
            InternalCalendarCacheEntry cachedEntry;
            if (cache.TryGetValue(id, out cachedEntry))
            {
                if (cachedEntry.IsExpired)
                    cache.TryRemove(id, out cachedEntry);
                else
                    return new CalendarCacheEntry(cachedEntry);
            }

            return null;
        }

        public CalendarCacheEntry PutCalendar(string id, CalendarModel calendarModel)
        {
            var cacheEntry = new InternalCalendarCacheEntry
            {
                CalendarModel = calendarModel,
                //ExpirationTime = DateTime.UtcNow.AddMinutes(5),
                ExpirationTime = DateTime.UtcNow.AddSeconds(20),
            };
            var internalCacheEntry = cache.AddOrUpdate(id, cacheEntry, (id1, entry) =>
            {
                // No changes (at least in number of appointments :))
                if (entry.CalendarModel.Appointments.Count() != cacheEntry.CalendarModel.Appointments.Count())
                    cacheEntry.CalendarModel.LastChangeDate = DateTime.UtcNow;
                    
                return cacheEntry;
            });

            return new CalendarCacheEntry(internalCacheEntry) { Id = id };
        }
    }

    public class CalendarCacheEntry : BaseCalendarCacheEntry
    {
        public string Id { get; set; }

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
        public bool IsExpired => ExpirationTime <= DateTime.UtcNow;
    }
}
