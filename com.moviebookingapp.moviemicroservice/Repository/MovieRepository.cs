using com.moviebookingapp.moviemicroservice.Collection;
using com.moviebookingapp.moviemicroservice.Models;
using MongoDB.Driver;
using System.Linq;

namespace com.moviebookingapp.moviemicroservice.Repository
{
    public class MovieRepository:IMovieRepository
    {
        private readonly IMongoCollection<Movie> _movieCollection;
        private readonly IMongoCollection<Ticket> _ticketCollection;

        public MovieRepository(IMongoDatabase mongoDatabase)
        {
            _movieCollection = mongoDatabase.GetCollection<Movie>("Movie");
            _ticketCollection = mongoDatabase.GetCollection<Ticket>("Ticket");

        }
        /// <summary>
        /// Get all movies available
        /// </summary>
        /// <returns></returns>
        public async Task<List<Movie>> GetAllMovieAsync()
        {
            return await _movieCollection.Find(_ => true).ToListAsync();
        }
        /// <summary>
        /// Get movies by moviename
        /// </summary>
        /// <param name="moviename"></param>
        /// <returns></returns>
        public async Task<List<Movie>> GetByMovienameAsync(string moviename)
        {
            return await _movieCollection.Find(m=>m.MovieName==moviename).ToListAsync();
        }

        public async Task<Movie> GetByMovieandTheatrenameAsync(string MovieName,string TheatreName)
        {
            return await _movieCollection.Find(m => m.MovieName == MovieName && m.TheatreName == TheatreName).FirstOrDefaultAsync();
        }

        public async Task BookTicket(Ticket ticket)
        {
            await _ticketCollection.InsertOneAsync(ticket);
        }

        public async Task<List<Ticket>> GetBookedTicketDetails(string MovieName, string TheatreName)
        {
            return await _ticketCollection.Find(t=>t.MovieName==MovieName && t.TheatreName==TheatreName).ToListAsync();
           
        }

        public async Task<List<string>> GetSeats(string MovieName, string TheatreName)
        {
            var ticketsData = await _ticketCollection.Find(t => t.MovieName == MovieName && t.TheatreName == TheatreName).ToListAsync();
            return ticketsData.SelectMany(x => x.SeatNumber).ToList();
        }

        public async Task UpdateMovieDetails(Movie movie)
        {
            var movieFilter = Builders<Movie>.Filter
                .Eq(e => e.MovieName, movie.MovieName);
            var updateNoOfSeatsAlloted = Builders<Movie>.Update
            .Set(u => u.RemainingSeats, movie.RemainingSeats);
            await _movieCollection.UpdateOneAsync(movieFilter, updateNoOfSeatsAlloted);
        }

        public async Task DeleteMovieById(string id)
        {
            await _movieCollection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
