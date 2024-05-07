using DatabaseService.Data.Models;
using Task = System.Threading.Tasks.Task;

namespace DatabaseService.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<Project> CreateAsync(Project project);
    Task UpdateAsync(Project project);
    Task<bool> DeleteAsync(Project project);
    Task<Project> GetByIdAsync(long projectId);
    Task<Project> GetByNameAndCompanyAsync(string name, Company company);

    Task AddPerformer(ProjectPerformer performer);
    Task<ProjectPerformer> GetPerformerByEmployeeAndProjectAsync(Employee employee, Project project);

    Task<bool> PerformerExistsByEmail(string email);
    Task<List<Project>> GetAllByCompanyAsync(Company company);
}
