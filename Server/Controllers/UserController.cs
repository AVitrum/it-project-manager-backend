using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Data.Requests;
using Server.Data.Responses;
using Server.Services.Interfaces;

namespace Server.Controllers;

[Route("Server/[controller]")]
[ApiController]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPut("AddImage")]
    public async Task<ActionResult> AddImage([FromForm] AddFileRequest request)
    {
        if (request.Equals(null))
        {
            return BadRequest("Error");
        }

        await userService.SaveImageAsync(request.File);
        return Ok("Saved");
    }

    [HttpPut("AddInfo")]
    public async Task<ActionResult<string>> AddInfo(AddInfoRequest request)
    {
        await userService.AddInfoAsync(request);
        return "Added";
    }

    [HttpDelete("DeleteFile")]
    public async Task<ActionResult> DeleteImage()
    {
        var response = await userService.DeleteImageAsync();
        return response ? Ok(response) : BadRequest(response);
    }

    [HttpGet("Info")]
    public async Task<ActionResult<UserInfoResponse>> Info()
    {
        return Ok(await userService.ProfileAsync());
    }
}