using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatabaseService.Repositories.Implementations;

public class ProjectRepository(AppDbContext dbContext) : IProjectRepository
{
    public async Task<Project> CreateAsync(Project project)
    {
        await dbContext.Projects.AddAsync(project);
        await dbContext.SaveChangesAsync();

        return await GetByNameAsync(project.Name);
    }

    public async Task UpdateAsync(Project project)
    {
        dbContext.Projects.Update(project);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Project project)
    {
        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Project> GetByIdAsync(long projectId)
    {
        return await dbContext.Projects
            .Include(e => e.Creator)
            .Include(e => e.ProjectPerformers)!
            .ThenInclude(e => e.Employee)
            .ThenInclude(e => e.User)
            .Include(e => e.Company)
            .FirstOrDefaultAsync(e => e.Id == projectId) 
               ?? throw new EntityNotFoundException(nameof(Project));
    }

    public async Task<Project> GetByNameAsync(string name)
    {
        return await dbContext.Projects
            .Include(e => e.Creator)
            .Include(e => e.ProjectPerformers)!
            .ThenInclude(e => e.Employee)
            .ThenInclude(e => e.User)
            .Include(e => e.Company)
            .FirstOrDefaultAsync(e => e.Name == name) 
               ?? throw new EntityNotFoundException(nameof(Project));
    }

    public async Task AddPerformer(ProjectPerformer performer)
    {
        await dbContext.ProjectPerformers.AddAsync(performer);
        await dbContext.SaveChangesAsync();
    }

    public async Task<ProjectPerformer> GetPerformerByEmployeeAndProjectAsync(Employee employee, Project project)
    {
        return await dbContext.ProjectPerformers
            .Include(e => e.Employee)
            .ThenInclude(e => e.PositionInCompany)
            .FirstOrDefaultAsync(e => e.EmployeeId == employee.Id && e.ProjectId == project.Id)
            ?? throw new EntityNotFoundException(nameof(ProjectPerformer));
    }
    
    public async Task<bool> PerformerExistsByEmail(string email)
    {
        return await dbContext.ProjectPerformers
            .Include(e => e.Employee)
            .ThenInclude(e => e.User)
            .AnyAsync(e => e.Employee.User!.Email == email);
    }
}
