using DatabaseService.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services.Interfaces;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssignmentController(IAssignmentService assignmentService) : ControllerBase
{
    [HttpPost("{projectId:long}/createAssignment")]
    public async Task<IActionResult> CreateAssignment(long projectId, AssignmentDto request)
    {
        await assignmentService.CreateAssignment(projectId, request);
        return Ok(new
        {
            message = "Created"
        });
    }

    [HttpPut("{id:long}/update")]
    public async Task<IActionResult> UpdateAssignment(long id, AssignmentDto request)
    {
        await assignmentService.UpdateAssignment(id, request);
        return Ok(new
        {
            message = "Updated"
        });
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAssignment(long id)
    {
        return Ok(await assignmentService.GetAssignmentAsync(id));
    }

    [HttpGet("{projectId:long}/getAll")]
    public async Task<IActionResult> GetAllAssignments(long projectId)
    {
        return Ok(await assignmentService.GetAllAssignmentsAsync(projectId));
    } 
}