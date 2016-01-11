using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Microsoft.Exchange.WebServices.Data;

namespace SystemOut.CalandarApi.Controllers
{
    public class CalendarController : ApiController
    {
        [HttpGet]
        public CalendarModel Get(string id)
        {
            // TODO: Implement your own credential provider
            //var credentialProvider = new CredentialProviderMock();
            var credentialProvider = new CredentialProvider();
            var credentials = credentialProvider.GetCredentials(id);
            if (credentials == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ""));
            var ewsService = new ExchangeService
            {
                Credentials = new WebCredentials(credentials.Username, credentials.Password, credentials.Domain),
                Url = new Uri(credentials.ServiceUrl)
            };
            var week = ewsService.FindAppointments(WellKnownFolderName.Calendar,
                new CalendarView(DateTime.Today, DateTime.Today.AddDays(7)));

            return new CalendarModel
            {
                Owner = id,
                Appointments = week.Select(a => new AppointmentModel
                {
                    Subject = a.Subject,
                    StartTime = a.Start.ToUniversalTime(),
                    EndTime = a.End.ToUniversalTime(),
                    Duration = a.Duration,
                    IsPrivate = a.Sensitivity == Sensitivity.Private || a.Sensitivity == Sensitivity.Confidential
                })
            };
        }
    }
}
