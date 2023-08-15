using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using com.moviebookingapp.usermicroservice.Controllers;
using com.moviebookingapp.usermicroservice.Repository;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using com.moviebookingapp.usermicroservice.Models;
using com.moviebookingapp.usermicroservice.Collection;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using com.moviebookingapp.usermicroservice.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace com.moviebookingapp.tests
{
    [TestFixture]
    public class UserTests
    {
        Mock<IUserRepository> UserMock = new Mock<IUserRepository>();
        Mock<IConfiguration> userConfig = new Mock<IConfiguration>();
        Mock<ILogger<UserController>> userlog = new Mock<ILogger<UserController>>();
        Mock<IEmailService> emailmock = new Mock<IEmailService>();
        UserController usercontroller;
        [SetUp]
        public void SetUp()
        {
            usercontroller = new UserController(UserMock.Object, userConfig.Object,
                userlog.Object, emailmock.Object);
        }


        [Test]
        public async Task registerUserData_HttpRsponse201_whenUserRegistered()
        {
            //Arrange => variable init

            Users userdata = new Users()
            {
                Id = "",
                LoginId = "001",
                FirstName = "Aamir",
                LastName = "Nawab",
                UserName = "Aamir1999",
                Email = "amallick546@gmail.com",
                Password = "Aamir@123",
                ContactNumber = "8335817708",
                Role = "User",
            };
            UserMock.Setup(context => context.Register(userdata));

            //ACT - Handling => prøve Movie Data
            IActionResult result = await usercontroller.Post(userdata);

            //Assert - Check if data is going through correctly.
            IStatusCodeActionResult statusCodeResult = (IStatusCodeActionResult)result;
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(201));
        }

        [Test]
        public async Task registerUserData_HttpRsponse400_whenUserNotRegistered()
        {
            //Arrange => variable init

            Users userdata = new Users()
            {
                Id = "",
                LoginId = "001",
                FirstName = "Aamir",
                LastName = "Nawab",
                UserName = "Aamir1999",
                Email = "amallick546@gmail.com",
                Password = "Aamir@123",
                ContactNumber = "8335817708",
                Role = "User",
            };
            UserMock.Setup(context => context.Register(userdata));

            //ACT - Handling => prøve Movie Data
            IActionResult result = await usercontroller.Post(null);

            //Assert - Check if data is going through correctly.
            IStatusCodeActionResult statusCodeResult = (IStatusCodeActionResult)result;
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(400));
        }


        [Test]
        public async Task Get_HttpResponse200_WhenValidLogin()
        {
            // Arrange          
                userConfig.Setup(c => c["JWT:Key"]).Returns("nk0dYQs0MbyzBeKLlnmy");
                userConfig.Setup(c => c["JWT:Issuer"]).Returns("http://localhost:13315");
                userConfig.Setup(c => c["JWT:Audience"]).Returns("http://localhost:13315");

            Users userdata = new Users
            {
                Id = "",
                LoginId = "001",
                FirstName = "Aamir",
                LastName = "Nawab",
                UserName = "Aamir1999",
                Email = "amallick546@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Aamir@123"),
                ContactNumber = "8335817708",
                Role = "User"
            };

            UserMock.Setup(context => context.ValidateLoginUser(userdata.Email, userdata.LoginId))
                    .ReturnsAsync(userdata);
            usercontroller = new UserController(UserMock.Object, userConfig.Object,
        userlog.Object, emailmock.Object);

            var claims = new List<Claim>
            {

                new Claim(JwtRegisteredClaimNames.Email, userdata.Email),
                new Claim(ClaimTypes.Email, userdata.Email),
                new Claim(ClaimTypes.Role, userdata.Role)
            };



            var token = new JwtSecurityToken(
                issuer: "http://localhost:13315",
                audience: "http://localhost:13315",
                expires: DateTime.Now.AddHours(3), 
                claims: claims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("nk0dYQs0MbyzBeKLlnmy")),
                    SecurityAlgorithms.HmacSha256)

            );

            var tokenHandler = new JwtSecurityTokenHandler();

            var expectedTokenString = new JwtSecurityTokenHandler().WriteToken(token);
            var expirationDate = token.ValidTo;
            var username = userdata.UserName;
            var role = userdata.Role;
            var expectedToken = $@"{{ token = {expectedTokenString}, expirationDate = {expirationDate}, username = {username}, role = {role} }}";

            // Act
            IActionResult result = await usercontroller.Get(new LoginModel
            {
                UserName = userdata.UserName,
                Email = userdata.Email,
                LoginId = userdata.LoginId,
                Password = "Aamir@123" // Use the actual password for the test user
            });

            // Assert
            OkObjectResult okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            var actualToken = okResult.Value.ToString();
            Console.WriteLine("Expected Token:");
            Console.WriteLine(expectedToken);
            Console.WriteLine("Actual Token:");
            Console.WriteLine(actualToken);

            Assert.That(actualToken, Is.EqualTo(expectedToken));
        }

        [Test]
        public async Task Get_HttpResponse404_WhenInvalidLogin()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "invalid@example.com",
                LoginId = "invalidlogin",
                Password = "invalidpassword"
            };
            UserMock.Setup(repo => repo.ValidateLoginUser(loginModel.Email, loginModel.LoginId))
                    .ReturnsAsync((Users)null); // Login validation fails

            // Act
            IActionResult result = await usercontroller.Get(loginModel);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.AreEqual(404, notFoundResult.StatusCode);

            // Additional assertions if needed
        }

        [Test]
        public async Task Get_HttpResponse401_WhenInvalidPassword()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "amallick546@gmail.com",
                LoginId = "001",
                Password = "Aamir@123"
            };
            var logedinUser = new Users
            {
                Email = "amallick546@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("testpassword"),
                Role = "User"
                // ... other properties as needed
            };
            UserMock.Setup(repo => repo.ValidateLoginUser(loginModel.Email, loginModel.LoginId))
                    .ReturnsAsync(logedinUser);

            // Act
            IActionResult result = await usercontroller.Get(loginModel);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            var unauthorizedResult = (UnauthorizedObjectResult)result;
            Assert.AreEqual(401, unauthorizedResult.StatusCode);

            // Additional assertions if needed
        }

    }
}
