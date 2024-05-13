using DatabaseService.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services.Interfaces;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]/{projectId:long}")]
[Authorize]
public class AssignmentController(IAssignmentService assignmentService) : ControllerBase
{
    [HttpPost("createAssignment")]
    public async Task<IActionResult> CreateAssignment(long projectId, AssignmentDto request)
    {
        await assignmentService.CreateAssignment(projectId, request);
        return Ok(new
        {
            message = "Created"
        });
    }

    [HttpGet("getAll")]
    public async Task<IActionResult> GetAllAssignments(long projectId)
    {
        return Ok(await assignmentService.GetAllAssignmentsAsync(projectId));
    } 
}