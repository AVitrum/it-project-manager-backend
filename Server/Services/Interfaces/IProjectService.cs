using DatabaseService.Data.DTOs;
using Server.Payload.Responses;

namespace Server.Services.Interfaces;

public interface IProjectService
{
    Task CreateAsync(long companyId, ProjectDto projectDto);
    Task UpdateAsync(long projectId, ProjectDto projectDto);
    Task ChangeProjectImage(long projectId, IFormFile file);
    Task<ProjectResponse> GetProjectInfoAsync(long projectId);
    
    Task AddPerformerAsync(long projectId, PerformerDto performerDto);
}