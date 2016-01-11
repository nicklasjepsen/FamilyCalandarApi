using System;
using System.Linq;
using System.Web.Http;
using Microsoft.Exchange.WebServices.Data;

namespace SystemOut.CalandarApi.Controllers
{
    public class CalendarController : ApiController
    {
        [HttpGet]
        public CalendarModel Get(string id)
        {
            var credentialProvider = new CredentialProviderMock();
            var ewsService = new ExchangeService
            {
                // TODO: Set value
                //Credentials = new WebCredentials("user", "pass", "domain"),
                //Url = new Uri("ews url")
            };
            var week = ewsService.FindAppointments(WellKnownFolderName.Calendar,
                new CalendarView(DateTime.Today, DateTime.Today.AddDays(7)));

            return new CalendarModel
            {
                Owner = email,
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
