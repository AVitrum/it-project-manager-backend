using Microsoft.AspNetCore.Mvc;
using Server.Data.Requests;
using Server.Services.Interfaces;
using UserHelper;
using UserHelper.Payload.Requests;

namespace Server.Controllers;

[Route("server/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserCreationRequest request)
    {
        await authService.RegisterAsync(request);
        return Ok("Registered!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginRequest request)
    {
        return Ok(await authService.LoginAsync(request));
    }

    [HttpPost("sendToken")]
    public async Task<IActionResult> SendToken(TokenRequest request)
    {
        await authService.SendVerificationToken(request.Email!);
        return Ok("The token has been sent!");
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify(TokenRequest request)
    {
        await authService.Verify(request.Token!);
        return Ok("Verified");
    }
}