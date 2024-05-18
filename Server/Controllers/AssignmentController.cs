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

    [HttpPost("{assignmentId:long}/addPerformer")]
    public async Task<IActionResult> AddPerformer(long assignmentId, PerformerDto request)
    {
        await assignmentService.AddPerformer(assignmentId, request);
        return Ok(new
        {
            message = "Added"
        });
    }

    [HttpPut("{id:long}/toReview")]
    public async Task<IActionResult> ToReview(long id)
    {
        await assignmentService.ToReview(id);
        return Ok(new
        {
            message = "Ok"
        });
    }
    
    [HttpPut("{id:long}/returnAssignment")]
    public async Task<IActionResult> ReturnAssignment(long id)
    {
        await assignmentService.ReturnTask(id);
        return Ok(new
        {
            message = "Ok"
        });
    }
    
    [HttpPut("{id:long}/markAsCompleted")]
    public async Task<IActionResult> MarkAsCompleted(long id)
    {
        await assignmentService.MarkAsCompleted(id);
        return Ok(new
        {
            message = "Ok"
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

    [HttpPost("{id:long}/addFile")]
    public async Task<IActionResult> AddFile(long id, IFormFile file)
    {
        await assignmentService.AddFile(id, file);
        return Ok(new
        {
            message = "Uploaded"
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

    [HttpPost("{id:long}/addComment")]
    public async Task<IActionResult> AddComment(long id, CommentDto request)
    {
        await assignmentService.AddComment(id, request);
        return Ok(new
        {
            message = "Created"
        });
    }
}