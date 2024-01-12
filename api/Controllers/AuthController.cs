using api.Data.Models;
using api.Data.Requests;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService, IUserService userService) : ControllerBase
{
    [HttpPost("register")]
    public ActionResult<string> Register(UserCreationRequest request)
    {
        if (request.Equals(null) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Username))
        {
            return BadRequest("Email and Username are required!");
        }

        authService.CreateUser(UserCreationRequest.UserCreationRequestToUser(request));
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

        var token = authService.CreateToken(user);
        return Ok(token);
    }
}