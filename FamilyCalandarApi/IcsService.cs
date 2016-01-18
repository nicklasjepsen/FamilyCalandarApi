using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SystemOut.CalandarApi
{
    internal class IcsService : IIcsService
    {
        public IcsService()
        {
#if DEBUG
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
#endif
        }

        public async Task<IcsCalendarModel> GetIcsContent(string path, string etag)
        {
            using (var httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(etag) && etag.StartsWith("\"") && etag.EndsWith("\""))
                    httpClient.DefaultRequestHeaders.IfNoneMatch.Add(new EntityTagHeaderValue(etag));
                var httpResponse = await httpClient.GetAsync(path);
                // TODO: Handle 304 - NotModified
                var strResponse = await httpResponse.Content.ReadAsStringAsync();
                return new IcsCalendarModel
                {
                    ETag = httpResponse.Headers.ETag.Tag,
                    IcsLines = strResponse.Split(new[] { Environment.NewLine }, StringSplitOptions.None),
                };
            }
        }
    }
}
