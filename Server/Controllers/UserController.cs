using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Server.Data.Requests;
using Server.Data.Responses;
using Server.Services.Interfaces;
using UserHelper;
using UserHelper.Payload.Requests;
using ResetPasswordRequest = UserHelper.Payload.Requests.ResetPasswordRequest;

namespace Server.Controllers;

[Route("server/[controller]")]
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