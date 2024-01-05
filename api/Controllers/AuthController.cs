using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Data.Models;
using api.Data.Requests;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace api.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        [HttpGet, Authorize]
        public ActionResult<string> GetMyName()
        {
            return Ok(_userService.GetName());
        }

        [HttpPost("register")]
        public ActionResult<string> Register(UserCreationRequest request)
        {
            if (request.Equals(null) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest("Email and Username are required.");
            }

            try
            {
                _userService.CreateUser(UserCreationRequest.UserCreationRequestToUser(request));
                return Ok("Registered");
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException;

                return innerException is Npgsql.PostgresException { SqlState: "23505" } 
                    ? Conflict("Email or Username already exists.") 
                    : StatusCode(500, "Database error occurred.");
            }
        }


        [HttpPost("login")]
        public ActionResult<User> Login(UserLoginRequest request)
        {
            if (request.Equals(null))
            {
                return BadRequest("User not found");
            }

            try
            {
                var user = _userService.GetByUsername(request.Username);
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return BadRequest("Wrong password.");
                }

                var token = CreateToken(user);
                return Ok(token);
            }
            catch (ArgumentException e)
            {
                BadRequest(e.Message);
            }

            return StatusCode(500, "Something went wrong");
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim> {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, "Admin"),
                new(ClaimTypes.Role, "User"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }