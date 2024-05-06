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
}