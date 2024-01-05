using api.Data.Models;
using api.Data.Requests;
using api.Data.Responses;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TeamController : ControllerBase
{
    private readonly ITeamService _teamService;
    private readonly IUserService _userService;

    public TeamController(ITeamService teamService, IUserService userService)
    {
        _teamService = teamService;
        _userService = userService;
    }

    [HttpPost("create")]
    public ActionResult Create(TeamCreationRequest request)
    {
        _teamService.Create(request);
        return Ok("Created");
    }

    [HttpPost("{teamId:long}/{userId:long}")]
    public ActionResult AddUser(long teamId, long userId)
    {
        var team = _teamService.Get(teamId);
        var user = _userService.GetById(userId);

        var added = _teamService.AddUser(user, team);
        return added ? Ok("Added") : BadRequest("Error");
    }

    [HttpGet("{teamId:long}")]
    public ActionResult GetById(long teamId) 
    {
        try 
        {
             var team = _teamService.Get(teamId);
             return Ok(TeamResponse.TeamToTeamResponse(team));
        }
        catch(ArgumentException e)
        {
            BadRequest(e.Message);
        }
        return BadRequest("Server error");;
    }
}