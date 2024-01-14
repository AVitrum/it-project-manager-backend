using api.Data.Requests;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<string>> Register(UserCreationRequest request)
    {
        await authService.RegisterAsync(request);
        return Ok("Registered!");
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(UserLoginRequest request)
    {
        return Ok(await authService.LoginAsync(request));
    }
}