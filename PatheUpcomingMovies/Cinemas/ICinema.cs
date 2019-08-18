using System;
namespace PatheUpcomingMovies.Cinemas
{
    public interface ICinema
    {
        string Site { get; }
        string Address { get; }
        double Latitude { get; }
        double Longitude { get; }
    }
}
