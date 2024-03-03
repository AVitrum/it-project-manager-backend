using EmailSystem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Data.Requests;
using Server.Data.Responses;
using Server.Services.Interfaces;
using UserHelper;

namespace Server.Controllers;

[Route("Server/[controller]")]
[ApiController]
[Authorize]
public class UserController(IUserService userService, IEmailSender emailSender) : ControllerBase
{
    [HttpPut("ChangePassword")]
    public async Task<ActionResult<string>> ChangePassword(ChangePasswordRequest request)
    {
        return Ok(await userService.ChangePasswordAsync(request));
    }

    [HttpGet("Info")]
    public async Task<ActionResult<UserInfoResponse>> Info()
    {
        return Ok(await userService.ProfileAsync());
    }

    [HttpPost("SendCode")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> SendCode(CodeRequest request)
    {
        var code = userService.GenerateCodeAsync(request.Email);
        await emailSender.SendEmailAsync(request.Email, "Confirmation code", code);
        return Ok("Sent");
    }
}