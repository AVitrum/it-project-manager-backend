using api.Data.Requests;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TeamController(ITeamService teamService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<ActionResult> Create(TeamCreationRequest request)
    {
        await teamService.CreateAsync(request);
        return Ok("Created");
    }

    [HttpPost("{teamId:long}/{userId:long}")]
    public async Task<ActionResult> AddUser(long teamId, long userId)
    {
        await teamService.AddUserAsync(teamId, userId);
        return Ok("Added");
    }

    [HttpGet("{teamId:long}")]
    public async Task<ActionResult> GetById(long teamId)
    {
        return Ok(await teamService.GetAsync(teamId));
    }
}