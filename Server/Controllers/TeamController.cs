using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Data.Requests;
using Server.Services.Interfaces;

namespace Server.Controllers;

[Route("Server/[controller]")]
[ApiController]
[Authorize]
public class TeamController(ITeamService teamService) : ControllerBase
{
    [HttpPost("Create")]
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