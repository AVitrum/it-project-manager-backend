using Microsoft.AspNetCore.Mvc;
using Server.Data.Requests;
using Server.Services.Interfaces;

namespace Server.Controllers;

[Route("Server/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("Register")]
    public async Task<ActionResult<string>> Register(UserCreationRequest request)
    {
        await authService.RegisterAsync(request);
        return Ok("Registered!");
    }

    [HttpPost("Login")]
    public async Task<ActionResult<string>> Login(UserLoginRequest request)
    {
        return Ok(await authService.LoginAsync(request));
    }
}