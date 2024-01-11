using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Data.Models;
using api.Data.Requests;
using api.Data.Responses;
using api.Data.SubModels;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace api.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IConfiguration configuration, IUserService userService) : ControllerBase
    {
        [HttpGet("info"), Authorize]
        public ActionResult<UserInfoResponse> Info()
        {
            return Ok(UserInfoResponse.UserToUserInfoResponse(userService.GetFromToken()));
        }

        [HttpPut("addInfo"), Authorize]
        public ActionResult<string> AddInfo(AddInfoRequest request)
        {
            var user = userService.GetFromToken();

            var additionalUserInfo = new AdditionalUserInfo
            {
                UserId = user.Id,
                Type = request.Type,
                Info = request.Info,
            };

            userService.AddInfo(additionalUserInfo);
            return "Added";
        }

        [HttpPost("register")]
        public ActionResult<string> Register(UserCreationRequest request)
        {
            if (request.Equals(null) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest("Email and Username are required!");
            }

            userService.CreateUser(UserCreationRequest.UserCreationRequestToUser(request));
            return Ok("Registered!");
        }

        [HttpPost("login")]
        public ActionResult<User> Login(UserLoginRequest request)
        {
            var user = userService.GetByUsername(request.Username);
            
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Wrong password.");
            }

            var token = CreateToken(user);
            return Ok(token);
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim> {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, "Admin"),
                new(ClaimTypes.Role, "User"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration.GetSection("AppSettings:Token").Value!));

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