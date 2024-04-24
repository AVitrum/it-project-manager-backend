using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Payload.Requests;
using Server.Services.Interfaces;
using ResetPasswordRequest = Server.Payload.Requests.ResetPasswordRequest;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("getResetPasswordToken")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateResetPasswordToken(TokenRequest request)
    {
        await userService.CreateResetPasswordTokenAsync(request.Email!);
        return Ok("The token has been sent!");
    }
    
    [HttpPut("resetPassword")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        await userService.ResetPassword(request);
        return Ok("Changed!");
    }
    
    
    [HttpPut("changePassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        return Ok(await userService.ChangePasswordAsync(request));
    }

    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        return Ok(await userService.ProfileAsync());
    }
}