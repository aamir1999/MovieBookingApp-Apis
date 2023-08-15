using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using com.moviebookingapp.moviemicroservice.Controllers;
using com.moviebookingapp.moviemicroservice.Repository;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using com.moviebookingapp.moviemicroservice.Collection;
using com.moviebookingapp.moviemicroservice.Models;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;

namespace com.moviebookingapp.tests
{
    [TestFixture]
    public class MovieTests
    {
        Mock<IMovieRepository> MovieMock = new Mock<IMovieRepository>();
        Mock<ILogger<MovieController>> MovieLog = new Mock<ILogger<MovieController>>();
        MovieController movieController;
        [SetUp]
        public void SetUp()
        {
            movieController = new MovieController(MovieMock.Object,MovieLog.Object);
        }

        [Test]
        public async Task getAllMovies_HttpRsponse200_whenDataExists()
        {
            //Arrange => variable init
            List<Movie> movieList = new List<Movie>();
            movieList.Add(new Movie());
            movieList.Add(new Movie());
            movieList.Add(new Movie());
            MovieMock.Setup(context => context.GetAllMovieAsync()).ReturnsAsync(movieList);

            //ACT - Handling => prøve Movie Data
            IActionResult result = await movieController.Get();

            //Assert - Check if data is going through correctly.
            IStatusCodeActionResult statusCodeResult = (IStatusCodeActionResult)result;
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task getAllMovies_HttpRsponse204_whenNoDataExists()
        {
            //Arrange  => variable init
           
            List<Movie> movieList = new List<Movie>();
            MovieMock.Setup(context => context.GetAllMovieAsync()).ReturnsAsync(movieList);

            //ACT - Handling => prøve Movie Data
            IActionResult result = await movieController.Get();

            //Assert - Check if data is going through correctly.
            IStatusCodeActionResult statusCodeResult = (IStatusCodeActionResult)result;
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public async Task getMovieByName_HttpRsponse200_whenDataExists()
        {
            //Arrange  => variable init
            var moviename = "Barbie";
            List<Movie> movieList = new List<Movie>();
            movieList.Add(new Movie { MovieName = "Barbie", TheatreName = "Inox", NoOfSeatsAlloted = 200 });
            MovieMock.Setup(context => context.GetByMovienameAsync(moviename)).ReturnsAsync(movieList);

            //ACT - Handling => prøve Movie Data
            IActionResult result = await movieController.Get(moviename);

            //Assert - Check if data is going through correctly.
            IStatusCodeActionResult statusCodeResult = (IStatusCodeActionResult)result;
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(200));
        }
        [Test]
        public async Task getMovieByName_HttpRsponse404_whenDataDoesntExists()
        {
            //Arrange  => variable init
            var moviename = "Oppenheimer";
            List<Movie> movieList = new List<Movie>();
            MovieMock.Setup(context => context.GetByMovienameAsync(moviename)).ReturnsAsync(movieList);

            //ACT - Handling => prøve Movie Data
            IActionResult result = await movieController.Get(moviename);

            //Assert - Check if data is going through correctly.
            IStatusCodeActionResult statusCodeResult = (IStatusCodeActionResult)result;
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task bookTicket_shouldReturn201_WhenTicketBooked()
        {
            //Arrange  => variable init
            // objectController and mock data
            Ticket bookMovieTicket = new Ticket()
            {
                Email = "user1@gmail.com",
                MovieName = "Barbie",
                TheatreName = "Inox",
                NumberOfTickets = 1,
                SeatNumber = new List<string> {"G1"},
            };
            MovieMock.Setup(context => context.BookTicket(bookMovieTicket));

            //ACT - Handling => prøve Movie Data
            IActionResult result = await movieController.Post(bookMovieTicket);

            //Assert - Check if data is going through correctly.
            IStatusCodeActionResult statusCodeResult = (IStatusCodeActionResult)result;
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(201));
        }

        [Test]
        public async Task bookTicket_shouldReturn400_WhenNoTicketBooked()
        {
            //Arrange  => variable init
            // objectController and mock data
            Ticket bookMovieTicket = new Ticket()
            {
                Email = "user1@gmail.com",
                MovieName = "Barbie",
                TheatreName = "Inox",
                NumberOfTickets = 1,
                SeatNumber = new List<string> { "G1"},
            };
            MovieMock.Setup(context => context.BookTicket(bookMovieTicket));

            //ACT - Handling => prøve Movie Data
            IActionResult result = await movieController.Post(null);

            //Assert - Check if data is going through correctly.
            IStatusCodeActionResult statusCodeResult = (IStatusCodeActionResult)result;
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(400));
        }

    }      
}
