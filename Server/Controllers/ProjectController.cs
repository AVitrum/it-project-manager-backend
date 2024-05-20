using DatabaseService.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services.Interfaces;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProjectController(IProjectService projectService) : ControllerBase
{
    [HttpPost("{companyId:long}/createProject")]
    public async Task<IActionResult> Create(long companyId, ProjectDto request)
    {
        await projectService.CreateAsync(companyId, request);
        return Ok(new
        {
            message = "Created"
        });
    }
    
    [HttpPut("{projectId:long}/image")]
    public async Task<IActionResult> UploadImage(long projectId, [FromForm] IFormFile file)
    {
        await projectService.ChangeProjectImage(projectId, file);
        return Ok();
    }
    
    [HttpPost("{projectId:long}/addPerformer")]
    public async Task<IActionResult> AddPerformer(long projectId, PerformerDto request)
    {
        await projectService.AddPerformerAsync(projectId, request);
        return Ok(new
        {
            message = "Added"
        });
    }
    
    [HttpDelete("{projectId:long}/removePerformer")]
    public async Task<IActionResult> RemovePerformer(long projectId, PerformerDto request)
    {
        await projectService.RemovePerformerAsync(projectId, request);
        return Ok(new
        {
            message = "Removed"
        });
    }

    [HttpPut("{projectId:long}/update")]
    public async Task<IActionResult> UpdateProject(long projectId, ProjectDto request)
    {
        await projectService.UpdateAsync(projectId, request);
        return Ok(new
        {
            message = "Performed"
        });
    }

    [HttpDelete("{projectId:long}/delete")]
    public async Task<IActionResult> DeleteProject(long projectId)
    {
        await projectService.DeleteAsync(projectId);
        return Ok(new
        {
            message = "Deleted"
        });
    }

    [HttpGet("{projectId:long}")]
    public async Task<IActionResult> GetProject(long projectId)
    {
        return Ok(await projectService.GetProjectInfoAsync(projectId));
    }
}