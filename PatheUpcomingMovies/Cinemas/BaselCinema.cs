using System;

namespace PatheUpcomingMovies.Cinemas
{
    public struct BaselCinema : ICinema
    {
        public string Site => "KUC";

        public string Address => "Pathé Küchlin\nSteinenvorstadt 55, 4051 Basel, Schweiz";

        public double Latitude => 47.5524961;

        public double Longitude => 7.5862286;
    }
}
