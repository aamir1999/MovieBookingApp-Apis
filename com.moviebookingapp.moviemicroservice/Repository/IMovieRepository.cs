using com.moviebookingapp.moviemicroservice.Collection;
using com.moviebookingapp.moviemicroservice.Models;

namespace com.moviebookingapp.moviemicroservice.Repository
{
    public interface IMovieRepository
    {
        Task<List<Movie>> GetAllMovieAsync();
        Task<List<Movie>> GetByMovienameAsync(string moviename);
        Task<Movie> GetByMovieandTheatrenameAsync(string MovieName, string TheatreName);
        Task BookTicket(Ticket ticket);
        Task<List<Ticket>> GetBookedTicketDetails(string MovieName, string TheatreName);
        Task<List<string>> GetSeats(string MovieName, string TheatreName);
        Task UpdateMovieDetails(Movie movie);
        Task DeleteMovieById(string id);



    }
}
