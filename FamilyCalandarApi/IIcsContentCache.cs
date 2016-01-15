using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemOut.CalandarApi.Controllers;

namespace SystemOut.CalandarApi
{
    public interface IIcsContentCache
    {
        bool ValidateCacheContent(string id, string[] newContent);

        void Put(string id, string[] newContent, ConcurrentDictionary<string, CalendarController.VEvent> contentToCache);

        ConcurrentDictionary<string, CalendarController.VEvent> GetCachedContent(string id);
    }

    public class IcsContentCache : IIcsContentCache
    {

        private readonly ConcurrentDictionary<string, string[]> icsContentCache;
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, CalendarController.VEvent>> parsedContentCache;

        public IcsContentCache()
        {
            icsContentCache = new ConcurrentDictionary<string, string[]>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newContent"></param>
        /// <returns>Returns true is cache is valid, false if not</returns>
        public bool ValidateCacheContent(string id, string[] newContent)
        {
            if (!icsContentCache.ContainsKey(id))
                return true;
            var oldContent = icsContentCache[id];
            return oldContent.SequenceEqual(newContent);
        }

        public void Put(string id, string[] newContent, ConcurrentDictionary<string, CalendarController.VEvent> contentToCache)
        {
            icsContentCache.AddOrUpdate(id, newContent, (s, strings) => newContent);
            parsedContentCache.AddOrUpdate(id, contentToCache, (s, events) => contentToCache);
        }

        public ConcurrentDictionary<string, CalendarController.VEvent> GetCachedContent(string id)
        {
            if (parsedContentCache.ContainsKey(id))
                return parsedContentCache[id];
            return null;
        }
    }
}
