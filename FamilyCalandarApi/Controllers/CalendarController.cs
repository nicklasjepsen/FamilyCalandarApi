using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Exchange.WebServices.Data;
using SystemOut.Toolbox;

namespace SystemOut.CalandarApi.Controllers
{
    public class CalendarController : ApiController
    {
        private readonly ICredentialProvider credentialProvider;
        private readonly IIcsService icsService;

        public CalendarController()
        {
            credentialProvider = new CredentialProvider();
            icsService = new IcsService();
        }

        public CalendarController(ICredentialProvider credentialProvider, IIcsService icsService)
        {
            this.credentialProvider = credentialProvider;
            this.icsService = icsService;
        }

        [HttpGet]
        public CalendarModel Get(string id)
        {
            try
            {
                // TODO: Implement your own credential provider
                var credentials = credentialProvider.GetCredentials(id);
                if (credentials == null)
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ""));

                switch (credentials.Type)
                {
                    case "EWS":
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
                    case "ICS":
                        var icsCal = GetIcsCalendar(credentials.ServiceUrl);
                        return new CalendarModel
                        {
                            Owner = id,
                            Appointments = icsCal.Where(a => a != null && a.StartDate.Date >= DateTime.UtcNow.Date && a.EndDate < DateTime.UtcNow.Date.AddDays(8))
                            .Select(e => new AppointmentModel
                            {
                                Subject = e.Summary,
                                StartTime = e.StartDate,
                                EndTime = e.EndDate,
                            })
                        };
                    default: throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ""));
                }
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }
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


        private IEnumerable<VEvent> GetIcsCalendar(string url)
        {
            var allLines = icsService.GetIcsContent(url);
            var events = new List<VEvent>();
            Parallel.For(0, allLines.Length, x =>
            {
                if (allLines[x] == "BEGIN:VEVENT")
                {
                    var y = x + 1;
                    var props = new Dictionary<string, string>();
                    do
                    {
                        var line = allLines[y];
                        y++;
                        if (!line.Contains(":"))
                            continue;
                        var splitterIndex = line.IndexOf(':');
                        var key = line.Substring(0, splitterIndex);
                        var value = line.Substring(splitterIndex + 1, line.Length - splitterIndex - 1);
                        if (props.ContainsKey(key))
                            continue;
                        else
                        {
                            if (key.Contains("DTSTART;TZID="))
                            {
                                // Handle timezone different
                                // First get timezone
                                var splitted = key.Split('=');
                                if (splitted.Length > 1)
                                {
                                    var tzStr = splitted[1];
                                    var tz = Converter.

                                }
                            }
                        props.Add(key, value);
                        }
                    } while (allLines[y] != "END:VEVENT");
                    var vevent = new VEvent();
                    string created, summary, startTime, endTime, sequence, uid;
                    if (props.TryGetValue("CREATED", out created))
                    {
                        // 20141110T180231Z
                        vevent.Created = DateTime.ParseExact(created, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);
                    }
                    if (props.TryGetValue("SUMMARY", out summary))
                    {
                        vevent.Summary = summary;
                    }
                    // TODO: Handle timezone
                    if (props.TryGetValue())
                    if (props.TryGetValue("DTSTART;TZID=Europe/Copenhagen", out startTime))
                    {
                        vevent.StartDate = DateTime.ParseExact(startTime, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture);
                    }
                    else if (props.TryGetValue("DTSTART;VALUE=DATE", out startTime))
                    {
                        vevent.StartDate = DateTime.ParseExact(startTime, "yyyyMMdd", CultureInfo.InvariantCulture);
                    }
                    if (props.TryGetValue("DTEND;TZID=Europe/Copenhagen", out endTime))
                    {
                        vevent.EndDate = DateTime.ParseExact(endTime, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture);
                    }
                    else if (props.TryGetValue("DTEND;VALUE=DATE", out endTime))
                    {
                        vevent.EndDate = DateTime.ParseExact(endTime, "yyyyMMdd", CultureInfo.InvariantCulture);
                    }
                    if (props.TryGetValue("SEQUENCE", out sequence))
                    {
                        vevent.Sequence = int.Parse(sequence);
                    }
                    if (props.TryGetValue("UID", out uid))
                    {
                        vevent.Uid = uid;
                    }
                    events.Add(vevent);
                }
            });

            return events;
        }

        class VEvent
        {
            public DateTime Created { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime StartDate { get; set; }
            public int Sequence { get; set; }
            public string Summary { get; set; }
            public string Uid { get; set; }
        }
    }
}
