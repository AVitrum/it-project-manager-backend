using api.Data.Enums;
using api.Data.Requests;
using api.Data.Responses;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TeamController(ITeamService teamService, IUserService userService) : ControllerBase
{
    [HttpPost("create")]
    public ActionResult Create(TeamCreationRequest request)
    {
        teamService.Create(request, userService.GetFromToken());
        return Ok("Created");
    }

    [HttpPost("{teamId:long}/{userId:long}")]
    public ActionResult AddUser(long teamId, long userId)
    {
        var team = teamService.Get(teamId);

        if (!teamService.HasPermission(userService.GetFromToken(), team))
        {
            return BadRequest("Server error.");
        }
        
        var target = userService.GetById(userId);
        var added = teamService.AddUser(target, team, UserRole.Regular);
        return added ? Ok("Added.") : BadRequest("Cannot add the user.");

    }

    [HttpGet("{teamId:long}")]
    public ActionResult GetById(long teamId) 
    {
        var team = teamService.Get(teamId);
        return Ok(TeamResponse.TeamToTeamResponse(team));
    }
}