using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Exchange.WebServices.Data;

namespace SystemOut.CalandarApi.Controllers
{
    public class CalendarController : ApiController
    {
        private readonly ICredentialProvider credentialProvider;
        private readonly IIcsService icsService;
        private readonly ICalendarCache calendarCache;


        public CalendarController(ICredentialProvider credentialProvider, IIcsService icsService, ICalendarCache calendarCache)
        {
            this.credentialProvider = credentialProvider;
            this.icsService = icsService;
            this.calendarCache = calendarCache;
        }

        [HttpGet]
        [Route("Ping/{message}")]
        public string Ping(string message)
        {
            return message;
        }

        [HttpGet]
        [Route("Calendar/{id}/{days}")]
        public async Task<CalendarModel> Get(string id, int days)
        {
            var credentials = credentialProvider.GetCredentials(id);
            if (credentials == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ""));
            
            var calendarModel = new CalendarModel
            {
                Owner = id,
            };

            switch (credentials.Type)
            {
                case "EWS":
                    var ewsService = new ExchangeService
                    {
                        Credentials = new WebCredentials(credentials.Username, credentials.Password, credentials.Domain),
                        Url = new Uri(credentials.ServiceUrl)
                    };
                    var week = ewsService.FindAppointments(WellKnownFolderName.Calendar,
                        new CalendarView(DateTime.Today, DateTime.Today.AddDays(days)));

                    calendarModel.Appointments = week.Select(a => new AppointmentModel
                    {
                        Subject = a.Subject,
                        StartTime = a.Start.ToUniversalTime(),
                        EndTime = a.End.ToUniversalTime(),
                        Duration = a.Duration,
                        IsPrivate =
                            a.Sensitivity == Sensitivity.Private || a.Sensitivity == Sensitivity.Confidential
                    });
                    break;
                case "ICS":
                    var cache = calendarCache.GetCalendar(id);
                    if (cache != null)
                    {
                        var icsServiceResponse = await icsService.GetIcsContent(credentials.ServiceUrl, cache.ETag);
                        if (icsServiceResponse.NotModified)
                        {
                            calendarModel.Appointments = cache.CalendarModel.Appointments;
                        }
                        else
                        {
                            calendarModel.Appointments = icsServiceResponse.Appointments;
                            calendarCache.PutCalendar(id,
                                new CalendarCacheEntry(id) {CalendarModel = calendarModel, ETag = icsServiceResponse.ETag });
                        }
                    }
                    else
                    {
                        var icsResponse = await icsService.GetIcsContent(credentials.ServiceUrl, string.Empty);
                        calendarModel.Appointments = icsResponse.Appointments;
                        calendarCache.PutCalendar(id,
                            new CalendarCacheEntry(id) { CalendarModel = calendarModel, ETag = icsResponse.ETag});
                    }
                    break;
                default:
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ""));
            }

            // Now only return the appointments in the requested range
            return new CalendarModel
            {
                Owner = calendarModel.Owner,
                Appointments = calendarModel.Appointments
                    .Where(a => a != null &&
                                a.StartTime.Date >= DateTime.UtcNow.Date &&
                                a.EndTime <= DateTime.UtcNow.Date.AddDays(days))
            };
        }

        // OData support follows
        //[EnableQuery]
        //[HttpGet]
        //public IQueryable<AppointmentModel> Get(string id)
        //{
        //    try
        //    {
        //        // TODO: Implement your own credential provider
        //        var credentials = credentialProvider.GetCredentials(id);
        //        if (credentials == null)
        //            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ""));

        //        switch (credentials.Type)
        //        {
        //            case "EWS":
        //                var ewsService = new ExchangeService
        //                {
        //                    Credentials = new WebCredentials(credentials.Username, credentials.Password, credentials.Domain),
        //                    Url = new Uri(credentials.ServiceUrl)
        //                };
        //                var week = ewsService.FindAppointments(WellKnownFolderName.Calendar,
        //                    new CalendarView(DateTime.Today.AddDays(-30), DateTime.Today.AddDays(90)));

        //                return week.Select(a => new AppointmentModel
        //                {
        //                    Subject = a.Subject,
        //                    StartTime = a.Start.ToUniversalTime(),
        //                    EndTime = a.End.ToUniversalTime(),
        //                    Duration = a.Duration,
        //                    IsPrivate = a.Sensitivity == Sensitivity.Private || a.Sensitivity == Sensitivity.Confidential
        //                }).AsQueryable();
        //            case "ICS":
        //                var icsCal = GetIcsCalendar(credentials.ServiceUrl);
        //                return icsCal.Where(a => a != null && a.StartDate.Date >= DateTime.UtcNow.Date && a.EndDate < DateTime.UtcNow.Date.AddDays(8))
        //                    .Select(e => new AppointmentModel
        //                    {
        //                        Subject = e.Summary,
        //                        StartTime = e.StartDate,
        //                        EndTime = e.EndDate,
        //                    }).AsQueryable();
        //            default: throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ""));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
        //    }
        //}
    }
}
