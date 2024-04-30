using Microsoft.AspNetCore.Mvc;
using Server.Payload.Requests;
using Server.Services.Interfaces;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegistrationRequest request)
    {
        await authService.RegisterAsync(request);
        return Ok(new {message = "Registered!"});
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginRequest request)
    {
        return Ok(await authService.LoginAsync(request));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshRequest request)
    {
        return Ok(await authService.RefreshJwtAsync(request));
    }

    [HttpPost("sendToken")]
    public async Task<IActionResult> SendVerificationToken(TokenRequest request)
    {
        await authService.SendVerificationToken(request.Email!);
        return Ok(new 
        { 
            message = "The token has been sent!"
        });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyAccountAsync(TokenRequest request)
    {
        await authService.VerifyAccountAsync(request.Token!);
        return Ok(new 
        {
            message = "Verified"
        });
    }
}