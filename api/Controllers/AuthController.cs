using api.Data.Models;
using api.Data.Requests;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public ActionResult<string> Register(UserCreationRequest request)
    {
        authService.Register(request);
        return Ok("Registered!");
    }

    [HttpPost("login")]
    public ActionResult<User> Login(UserLoginRequest request)
    {
        return Ok(authService.Login(request));
    }
}