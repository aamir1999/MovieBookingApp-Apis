using com.moviebookingapp.moviemicroservice.Collection;
using com.moviebookingapp.moviemicroservice.Models;
using com.moviebookingapp.moviemicroservice.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace com.moviebookingapp.moviemicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieRepository _imovieRepository;
        private readonly ILogger<MovieController> _logger;
        public MovieController(IMovieRepository movieRepository, ILogger<MovieController> logger)
        {
            _imovieRepository = movieRepository;
            _logger = logger;
        }

        //Get all movies
        [Route("/api/v1.0/moviebooking/all")]
        [HttpGet, Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Get all movies available");
            var movies = await _imovieRepository.GetAllMovieAsync();
            if(movies.Count() == 0)
            {
                return NoContent();
            }
            return Ok(movies);
        }


        // Search By Moviename
        [Route("/api/v1.0/moviebooking/movies/search/moviename")]
        [HttpGet, Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Get([FromQuery]string moviename)
        {
            var movie = await _imovieRepository.GetByMovienameAsync(moviename);
            if (movie.Count == 0)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        // Book Tickets
        [Route("/api/v1.0/moviebooking/moviename/add")]
        [HttpPost, Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Post([FromBody] Ticket ticket)
        {
            _logger.LogInformation($"{ticket} is being booked");
            if (ticket == null)
            {
                return BadRequest();
            }
            var movie = _imovieRepository.GetByMovienameAsync(ticket.MovieName);
            if (movie == null)
            {
                return NotFound("No movie availabe with this name");
            }
           
            await _imovieRepository.BookTicket(ticket);
            return CreatedAtAction(nameof(Get), new { id = ticket.Id }, ticket);

        }

        // Get BookedMovie Tickets
        [Route("/api/v1.0/moviebooking/movies/getBookedMovieDetails")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetMovieRequest movie)
        {
            var bookedMovieDetails = await _imovieRepository.GetSeats(movie.MovieName, movie.TheatreName);
            if (bookedMovieDetails == null)
            {
                return NotFound("Ticket for above is not booked");
            }
            return Ok(bookedMovieDetails);
        }


        // Update Movie Tickets
        [Route("/api/v1.0/moviebooking/moviename/update/ticket")]
        [HttpPut, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put([FromBody] GetMovieRequest movie)
        {
            var movieDetails=await _imovieRepository.GetByMovieandTheatrenameAsync(movie.MovieName,movie.TheatreName);
            if (movieDetails == null)
            {
                return NotFound("No movie found");
            }
            var bookedMovieDetails =await _imovieRepository.GetBookedTicketDetails(movie.MovieName,movie.TheatreName);
            if(bookedMovieDetails==null)
            {
                return NotFound("Ticket for above is not booked");
            }
            var temp = movieDetails.NoOfSeatsAlloted;
            foreach(var bookedMovie in bookedMovieDetails)
            {
                temp -= bookedMovie.NumberOfTickets;
            }
            movieDetails.RemainingSeats=temp;
            await _imovieRepository.UpdateMovieDetails(movieDetails);
            return Ok(new { message = "Movie details updated successfully!!" });
        }

        

        // DELETE MOVIE
        [Route("/api/v1.0/moviebooking/moviename/delete/id")]
        [HttpDelete, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromQuery] GetMovieRequest movie)
        {
            var movieDetails = await _imovieRepository.GetByMovieandTheatrenameAsync(movie.MovieName, movie.TheatreName);
            if (movieDetails == null)
            {
                return NotFound();
            }
            await _imovieRepository.DeleteMovieById(movieDetails.Id);
            return Ok(new { message = "Movie Deleted Successfully" });
        }
    }
}
