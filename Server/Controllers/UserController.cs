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
    [HttpGet("profile")]
    public async Task<IActionResult> UserProfile()
    {
        return Ok(await userService.UserProfileAsync());
    }

    [HttpPut]
    public async Task<IActionResult> UpdateInfo(UserUpdateRequest request)
    {
        throw new NotImplementedException();
    }

    [HttpPut("profileImage")]
    public async Task<IActionResult> ProfileImage([FromForm] IFormFile file)
    {
        await userService.ChangeProfileImage(file);
        return Ok();
    }

    [HttpPost("sendResetPasswordToken")]
    [AllowAnonymous]
    public async Task<IActionResult> SendResetPasswordToken(TokenRequest request)
    {
        await userService.CreateResetPasswordTokenAsync(request.Email!);
        return Ok(new 
        {
            message = "The token has been sent!"
        });
    }

    [HttpPut("resetPassword")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        await userService.ResetPasswordAsync(request);
        return Ok(new 
        {
            message = "Changed!"
        });
    }


    [HttpPut("changePassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        return Ok(new {message = await userService.ChangePasswordAsync(request)});
    }
}