using com.moviebookingapp.usermicroservice.Collection;
using com.moviebookingapp.usermicroservice.Models;
using com.moviebookingapp.usermicroservice.Repository;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Text;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using com.moviebookingapp.usermicroservice.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace com.moviebookingapp.usermicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _iuserRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;
        private readonly IEmailService _emailService;


        public UserController(IUserRepository iuserRepository, 
            IConfiguration configuration,
            ILogger<UserController> logger,
            IEmailService emailService)
        {
            _iuserRepository = iuserRepository;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;

        }



        // Login 
        [HttpGet]
        [Route("/api/v1.0/moviebooking/login")]
        public async Task<IActionResult> Get([FromQuery] LoginModel loginModel)
        {
            _logger.LogInformation("User begins to login");
            var logedinUser = await _iuserRepository.ValidateLoginUser(loginModel.Email, loginModel.LoginId);
            if (logedinUser == null)
            {
                return NotFound("No user found with the given credentials");
            }
            var verifyPassword = BCrypt.Net.BCrypt.Verify(loginModel.Password, logedinUser.Password);
            if (!verifyPassword)
            {
                return Unauthorized("Invalid Password");
            }
            var claims = new List<Claim>
            {

                  new Claim(JwtRegisteredClaimNames.Email,loginModel.Email ),
                  new Claim(ClaimTypes.Email,loginModel.Email ),
                  new Claim(ClaimTypes.Role,logedinUser.Role)

            };
            var token = GenerateAccessToken(claims);
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expirationDate = token.ValidTo,
                username=logedinUser.UserName,
                role=logedinUser.Role
            });
        }



        // register as a new user 
        [Route("/api/v1.0/moviebooking/register")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Users users)
        {
            _logger.LogInformation("Seri Log is Working");
            if (users==null)
            {
                return BadRequest("Users data cannot be null");
            }
            var newUser = await _iuserRepository.ValidateUser(users.Email, users.LoginId);
            if (newUser != null)
            {
                return BadRequest("Userdetails already exists");
            }
            users.Password = BCrypt.Net.BCrypt.HashPassword(users.Password);
            await _iuserRepository.Register(users);
            return CreatedAtAction(nameof(Get), new { id = users.Id }, users);
        }

        //Forgot password
        [HttpGet]
        [Route("/api/v1.0/moviebooking/forgot-password")]
        public async Task<IActionResult> Get([FromQuery] ForgotPasswordRequest forgotPasswordModel)
        {
            var user = await _iuserRepository.ForgotPassword(forgotPasswordModel.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // create reset token that expires after 1 day
            user.ResetToken = generateResetToken();
            user.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

            await _iuserRepository.UpdateResetToken(user);

            sendPasswordResetEmail(user);


            return Ok(new { message = "Please check your email for password reset instructions" });
        }

        // reset user password
        [Route("/api/v1.0/moviebooking/reset-password")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPassword)
        {
            var user = await _iuserRepository.ValidateResetToken(resetPassword.Token);
            if (user == null)
            {
                return NotFound("Invalid Token");
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(resetPassword.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpires = null;
            await _iuserRepository.UpdatePassword(user);
            return Ok(new { message = "Password reset successful, you can now login" });
        }

        /// <summary>
        /// GenerateToken for logedinUser
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        private JwtSecurityToken GenerateAccessToken(List<Claim> claims)
        {
            var token = new JwtSecurityToken(
                issuer: "http://localhost:13315",
                audience: "http://localhost:13315",
                expires: DateTime.Now.AddHours(3),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("nk0dYQs0MbyzBeKLlnmy")),
                    SecurityAlgorithms.HmacSha256)
            );

            return token;
        }



        /// <summary>
        /// Generate Reset Token
        /// </summary>
        /// <returns></returns>
        private string generateResetToken()
        {
            // token is a cryptographically strong random sequence of values
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            return token;
        }

        private void sendPasswordResetEmail(Users user)
        {
            string message;
                var resetUrl = $"http://localhost:4200/reset/?token={user.ResetToken}";
                message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                            <p><a href=""{resetUrl}"">{resetUrl}</a></p>";

            _emailService.Send(
                to: user.Email,
                subject: "Sign-up Verification API - Reset Password",
                html: $@"<h4>Reset Password Email</h4>
                        {message}"
            );
        }

       
    }
}
