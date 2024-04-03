using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services.Interfaces;
using UserService.Payload.Requests;
using ResetPasswordRequest = UserService.Payload.Requests.ResetPasswordRequest;

namespace Server.Controllers;

[Route("server/[controller]")]
[ApiController]
[Authorize]
public class EmployeeController(IEmployeeService employeeService) : ControllerBase
{
    [HttpGet("getResetPasswordToken")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateResetPasswordToken(TokenRequest request)
    {
        await employeeService.CreateResetPasswordTokenAsync(request.Email!);
        return Ok("The token has been sent!");
    }
    
    [HttpPut("resetPassword")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        await employeeService.ResetPassword(request);
        return Ok("Changed!");
    }
    
    
    [HttpPut("changePassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        return Ok(await employeeService.ChangePasswordAsync(request));
    }

    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        return Ok(await employeeService.ProfileAsync());
    }
}