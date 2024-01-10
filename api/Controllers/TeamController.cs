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
public class TeamController(ITeamService teamService, IUserService userService) : ControllerBase
{
    [HttpPost("create")]
    public ActionResult Create(TeamCreationRequest request)
    {
        try
        {
            teamService.Create(request, userService.GetFromToken());
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
        var team = teamService.Get(teamId);

        try
        {
            if (teamService.HasPermission(userService.GetFromToken(), team))
            {
                var target = userService.GetById(userId);
                var added = teamService.AddUser(target, team, UserRole.Regular);
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
             var team = teamService.Get(teamId);
             return Ok(TeamResponse.TeamToTeamResponse(team));
        }
        catch(ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }
}