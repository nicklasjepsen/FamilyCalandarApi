using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
            var response = new IcsCalendarModel
            {
                ETag = etag
            };
            using (var httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(etag) && etag.StartsWith("\"") && etag.EndsWith("\""))
                    httpClient.DefaultRequestHeaders.IfNoneMatch.Add(new EntityTagHeaderValue(etag));
                var httpResponse = await httpClient.GetAsync(path);
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    response.ETag = httpResponse.Headers.ETag.Tag;
                    response.Appointments = IcsParser.ParseAppointments((await httpResponse.Content.ReadAsStringAsync()).Split(new[] { Environment.NewLine }, StringSplitOptions.None));
                }
                else if (httpResponse.StatusCode == HttpStatusCode.NotModified)
                    response.NotModified = true;

                return response;
            }
        }
    }
}
