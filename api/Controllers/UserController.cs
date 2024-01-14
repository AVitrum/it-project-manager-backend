using api.Data.Requests;
using api.Data.Responses;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPut("addImage")]
    public async Task<ActionResult> AddImage([FromForm] AddFileRequest request)
    {
        if (request.Equals(null))
        {
            return BadRequest("Error");
        }

        await userService.SaveImageAsync(request.File);
        return Ok("Saved");
    }

    [HttpPut("addInfo")]
    public async Task<ActionResult<string>> AddInfo(AddInfoRequest request)
    {
        await userService.AddInfoAsync(request);
        return "Added";
    }

    [HttpDelete("deleteFile")]
    public async Task<ActionResult> DeleteImage()
    {
        var response = await userService.DeleteImageAsync();
        return response ? Ok(response) : BadRequest(response);
    }

    [HttpGet("info")]
    public async Task<ActionResult<UserInfoResponse>> Info()
    {
        return Ok(await userService.ProfileAsync());
    }
}