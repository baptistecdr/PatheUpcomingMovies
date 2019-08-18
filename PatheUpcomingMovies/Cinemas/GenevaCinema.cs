using System;
namespace PatheUpcomingMovies.Cinemas
{
    public class GenevaCinema : ICinema
    {
        public string Site => "BAL";

        public string Address => "Pathé Balexert & IMAX\nAvenue Louis Casaï 27, 1211 Geneva, Switzerland";

        public double Latitude => 46.219110;

        public double Longitude => 6.114466;
    }
}
