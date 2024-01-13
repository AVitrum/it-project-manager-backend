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
    public ActionResult Create(TeamCreationRequest request)
    {
        teamService.Create(request);
        return Ok("Created");
    }

    [HttpPost("{teamId:long}/{userId:long}")]
    public ActionResult AddUser(long teamId, long userId)
    {
        teamService.AddUser(teamId, userId);
        return Ok("Added");
    }

    [HttpGet("{teamId:long}")]
    public ActionResult GetById(long teamId) 
    {
        return Ok(teamService.Get(teamId));
    }
}