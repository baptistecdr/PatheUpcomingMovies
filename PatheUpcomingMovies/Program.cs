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

            [Option('l', "language", Required = false, Default = Language.Fr, HelpText = "Language of the informations. Valid values: De, En, Fr")]
            public Language Language { get; set; }

            [Option('o', "output", Required = false, Default = "./PatheUpcomingMovies.ics",  HelpText = "Path to export ics file.")]
            public string Output { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o => {
                ICinema cinema = null;
                switch (o.City)
                {
                    case City.Basel:
                        cinema = new BaselCinema();
                        break;
                    case City.Bern:
                        cinema = new BernCinema();
                        break;
                    case City.Geneva:
                        cinema = new GenevaCinema();
                        break;
                    case City.LausanneFlo:
                        cinema = new LausanneFloCinema();
                        break;
                    case City.LausanneGal:
                        cinema = new LausanneGalCinema();
                        break;
                    case City.Lucerne:
                        cinema = new LucerneCinema();
                        break;
                    case City.Spreitenbach:
                        cinema = new SpreitenbachCinema();
                        break;
                    case City.Zurich:
                        cinema = new ZurichCinema();
                        break;
                }

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

                detailResponse.Data.Response.Docs.ForEach((doc) =>
                {
                    CalendarEvent calendarEvent = new CalendarEvent
                    {
                        Summary = o.Language == Language.De ? doc.TitleDe : o.Language == Language.En ? doc.Title : doc.TitleFr,
                        Description = o.Language == Language.De ? doc.SynopsisDe : o.Language == Language.En ? doc.SynopsisEn : doc.SynopsisFr,
                        Start = new CalDateTime(o.City == City.Geneva || o.City == City.LausanneFlo || o.City == City.LausanneGal || !doc.ReleaseDeCh.HasValue ? doc.ReleaseFrCh.Value.DateTime : doc.ReleaseDeCh.Value.DateTime),
                        End = new CalDateTime(o.City == City.Geneva || o.City == City.LausanneFlo || o.City == City.LausanneGal || !doc.ReleaseDeCh.HasValue ? doc.ReleaseFrCh.Value.DateTime : doc.ReleaseDeCh.Value.DateTime),
                        Location = cinema.Address,
                        GeographicLocation = new GeographicLocation(cinema.Latitude, cinema.Longitude)
                    };
                    calendar.Events.Add(calendarEvent);
                });
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
