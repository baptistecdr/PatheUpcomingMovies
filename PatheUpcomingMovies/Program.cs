using System;
using RestSharp;
using System.Linq;
using System.IO;
using CommandLine;
using PatheUpcomingMovies.Cinemas;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Ical.Net.CalendarComponents;

namespace PatheUpcomingMovies
{
    class Program
    {
        public class Options
        {
            [Option('c', "city", Required = false, Default = City.Geneva, HelpText = "City of the cinema. Valid values: Basel, Bern, Geneva, LausanneFlo, LausanneGal, Lucern, Spreitenbach, Zurich")]
            public City City { get; set; }

            [Option('l', "language", Required = false, Default = Language.Fr, HelpText = "Language of synopsis. Valid values: De, En, Fr")]
            public Language Language { get; set; }

            [Option('t', "trailer-language", Required = false, Default = TrailerLanguage.Ov, HelpText = "Language of trailers. Valid values: De, Ov, Fr")]
            public TrailerLanguage TrailerLanguage { get; set; }

            [Option('v', "vimeo-bypass-url", Required = false, Default = "", HelpText = "Url to bypass Vimeo. Example: https://example.com/bypass_vimeo.php")]
            public string VimeoBypassUrl { get; set; }

            [Option('o', "output", Required = false, Default = "./PatheUpcomingMovies.ics",  HelpText = "Path to export ics file.")]
            public string Output { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o => {
                ICinema cinema = o.City switch
                {
                    City.Basel => new BaselCinema(),
                    City.Bern => new BernCinema(),
                    City.Geneva => new GenevaCinema(),
                    City.LausanneFlo => new LausanneFloCinema(),
                    City.LausanneGal => new LausanneGalCinema(),
                    City.Lucerne => new LucerneCinema(),
                    City.Spreitenbach => new SpreitenbachCinema(),
                    City.Zurich => new ZurichCinema(),
                    _ => new GenevaCinema()
                };

                /* Retrieve from pathe.ch upcoming movies */
                RestClient client = new RestClient("https://pathe.ch/solr/pathe-movies");
                IRestRequest upcomingRequest = new RestRequest("upcoming?{fq}", Method.GET).AddParameter("fq", $"site:({cinema.Site})");
                IRestResponse<UpcomingApi.Upcoming> upcomingResponse = client.Execute<UpcomingApi.Upcoming>(upcomingRequest);

                /* Maybe the user has no internet or an error occured during deserialization */
                if (!upcomingResponse.IsSuccessful || upcomingResponse.Data == null)
                {
                    Console.Error.WriteLine($"An error occured during the upcoming request ({upcomingResponse.ErrorMessage}).");
                    return;
                }

                if (upcomingResponse.Data.Response.Docs.Count == 0){
                    Console.Error.WriteLine($"No upcoming movie for the cinema in {o.City}.");
                    return;
                }

                /* Retrieve details for all upcoming movies */
                IRestRequest detailRequest = new RestRequest("detail?{fq}").AddParameter("fq", $"id:({String.Join(" ", upcomingResponse.Data.Response.Docs.Select(m => m.MovieId).ToArray())})");
                IRestResponse<DetailApi.Detail> detailResponse = client.Execute<DetailApi.Detail>(detailRequest);

                /* Maybe the user has no internet or an error occured during deserialization */
                if(!detailResponse.IsSuccessful || detailResponse.Data == null)
                {
                    Console.Error.WriteLine($"An error occured during the detail request ({detailResponse.ErrorMessage}).");
                    return;
                }

                if(detailResponse.Data.Response.Docs.Count == 0)
                {
                    Console.Error.WriteLine($"No description retrieved for {o.City} cinema films.");
                    return;
                }

                Calendar calendar = new Calendar();
                foreach (DetailApi.Doc doc in detailResponse.Data.Response.Docs)
                {
                    CalendarEvent calendarEvent = new CalendarEvent();
                    switch (o.Language)
                    {
                        case Language.De:
                            calendarEvent.Summary = doc.TitleDe;
                            calendarEvent.Description = doc.SynopsisDe;
                            break;
                        case Language.En:
                            calendarEvent.Summary = doc.Title;
                            calendarEvent.Description = doc.SynopsisEn;
                            break;
                        case Language.Fr:
                            calendarEvent.Summary = doc.TitleFr;
                            calendarEvent.Description = doc.SynopsisFr;
                            break;
                    };
                    switch (o.City)
                    {
                        case City.Basel:
                            calendarEvent.Start = new CalDateTime(doc.ReleaseDeCh.Value.DateTime);
                            calendarEvent.End = new CalDateTime(doc.ReleaseDeCh.Value.DateTime);
                            break;
                        case City.Bern:
                            calendarEvent.Start = new CalDateTime(doc.ReleaseDeCh.Value.DateTime);
                            calendarEvent.End = new CalDateTime(doc.ReleaseDeCh.Value.DateTime);
                            break;
                        case City.Geneva:
                            calendarEvent.Start = new CalDateTime(doc.ReleaseFrCh.Value.DateTime);
                            calendarEvent.End = new CalDateTime(doc.ReleaseFrCh.Value.DateTime);
                            break;
                        case City.LausanneFlo:
                            calendarEvent.Start = new CalDateTime(doc.ReleaseFrCh.Value.DateTime);
                            calendarEvent.End = new CalDateTime(doc.ReleaseFrCh.Value.DateTime);
                            break;
                        case City.LausanneGal:
                            calendarEvent.Start = new CalDateTime(doc.ReleaseFrCh.Value.DateTime);
                            calendarEvent.End = new CalDateTime(doc.ReleaseFrCh.Value.DateTime);
                            break;
                        case City.Lucerne:
                            calendarEvent.Start = new CalDateTime(doc.ReleaseDeCh.Value.DateTime);
                            calendarEvent.End = new CalDateTime(doc.ReleaseDeCh.Value.DateTime);
                            break;
                        case City.Spreitenbach:
                            calendarEvent.Start = new CalDateTime(doc.ReleaseDeCh.Value.DateTime);
                            calendarEvent.End = new CalDateTime(doc.ReleaseDeCh.Value.DateTime);
                            break;
                        case City.Zurich:
                            calendarEvent.Start = new CalDateTime(doc.ReleaseDeCh.Value.DateTime);
                            calendarEvent.End = new CalDateTime(doc.ReleaseDeCh.Value.DateTime);
                            break;
                    }
                    calendarEvent.Location = cinema.Address;
                    calendarEvent.GeographicLocation = new GeographicLocation(cinema.Latitude, cinema.Longitude);
                    string vimeoId = o.TrailerLanguage switch
                    {
                        TrailerLanguage.De => !string.IsNullOrEmpty(doc.TrailerEmbedDe) ? doc.TrailerEmbedDe.Remove(0, 1) : "",
                        TrailerLanguage.Ov => !string.IsNullOrEmpty(doc.TrailerEmbedOv) ? doc.TrailerEmbedOv.Remove(0, 1) : "",
                        TrailerLanguage.Fr => !string.IsNullOrEmpty(doc.TrailerEmbedFr) ? doc.TrailerEmbedFr.Remove(0, 1) : "",
                        _ => ""
                    };
                    if (!string.IsNullOrEmpty(o.VimeoBypassUrl) && !string.IsNullOrEmpty(vimeoId))
                    {
                        calendarEvent.Url = new Uri($"{o.VimeoBypassUrl}?v={vimeoId}");
                    }

                    calendar.Events.Add(calendarEvent);
                }
                CalendarSerializer serializer = new CalendarSerializer();
                string serializedCalendar = serializer.SerializeToString(calendar);
                try
                {
                    File.WriteAllText(o.Output, serializedCalendar);
                    Console.WriteLine($"Calendar has been exported to '{o.Output}'.");
                }
                catch(Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                }
            });
        }
    }
}
