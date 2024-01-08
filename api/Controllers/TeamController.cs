using System.Security.Authentication;
using api.Data.Enums;
using api.Data.Requests;
using api.Data.Responses;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        try
        {
            _teamService.Create(request, _userService.GetFromToken());
            return Ok("Created");
        }
        catch (AuthenticationException e)
        {
            return BadRequest(e.Message);
        }
        catch (DbUpdateException ex)
        {
            var innerException = ex.InnerException;

            return innerException is Npgsql.PostgresException { SqlState: "23505" } 
                ? Conflict("A team with this name already exists.") 
                : StatusCode(500, "Database error occurred.");
        }
    }

    [HttpPost("{teamId:long}/{userId:long}")]
    public ActionResult AddUser(long teamId, long userId)
    {
        var team = _teamService.Get(teamId);

        try
        {
            if (_teamService.HasPermission(_userService.GetFromToken(), team))
            {
                var target = _userService.GetById(userId);
                var added = _teamService.AddUser(target, team, UserRole.Regular);
                return added ? Ok("Added.") : BadRequest("Cannot add the user.");
            }
        }
        catch (AuthenticationException e)
        {
            return BadRequest(e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        
        return BadRequest("Server error.");
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
        return BadRequest("Server error.");
    }
}