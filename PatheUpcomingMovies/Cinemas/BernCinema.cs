using System;
namespace PatheUpcomingMovies.Cinemas
{
    public class BernCinema : ICinema
    {
        public string Site => "WES";

        public string Address => "Pathé Bern\nRiedbachstrasse 102, 3027 Bern, Schweiz";

        public double Latitude => 46.9438522;

        public double Longitude => 7.3696732;
    }
}
