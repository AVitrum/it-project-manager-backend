using DatabaseService.Data.Models;

namespace DatabaseService.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<Project> CreateAsync(Project project);
    Task UpdateAsync(Project project);
    Task<bool> DeleteAsync(Project project);
    Task<Project> GetByIdAsync(long projectId);

    Task AddPerformer(ProjectPerformer performer);

    Task<bool> PerformerExistsByEmail(string email);
}
