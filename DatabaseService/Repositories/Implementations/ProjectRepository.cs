using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DatabaseService.Repositories.Implementations;

public class ProjectRepository(AppDbContext dbContext, IConfiguration configuration) : IProjectRepository
{
    public async Task<Project> CreateAsync(Project project)
    {
        await dbContext.Projects.AddAsync(project);
        await dbContext.SaveChangesAsync();

        return await GetByNameAndCompanyAsync(project.Name, project.CompanyId);
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
            .Include(e => e.ProjectPerformers)
            .ThenInclude(e => e.Employee)
            .ThenInclude(e => e.PositionInCompany)
            .Include(e => e.ProjectPerformers)
            .ThenInclude(e => e.Employee)
            .ThenInclude(e => e.User)
            .ThenInclude(e => e.ProfilePhoto)
            .FirstOrDefaultAsync(e => e.Id == projectId) 
               ?? throw new EntityNotFoundException(nameof(Project));
    }

    public async Task<Project> GetByNameAndCompanyAsync(string name, long companyId)
    {
        return await dbContext.Projects
            .Include(e => e.Creator)
            .Include(e => e.ProjectPerformers)
            .ThenInclude(e => e.Employee)
            .ThenInclude(e => e.User)
            .Include(e => e.Company)
            .FirstOrDefaultAsync(e => e.Name == name && e.CompanyId == companyId) 
               ?? throw new EntityNotFoundException(nameof(Project));
    }
    
    public async Task<List<Project>> GetAllByCompanyIdAsync(long companyId)
    {
        return await dbContext.Projects
            .Include(e => e.ProjectPerformers)
            .ThenInclude(e => e.Employee)
            .ThenInclude(e => e.User)
            .Include(e => e.Creator)
            .ThenInclude(e => e!.User)
            .Where(e => e.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<List<Project>> GetAllByCompanyIdSql(long companyId)
    {
        var projects =  await dbContext.Projects.Where(p => p.CompanyId == companyId).ToListAsync();
        return projects;
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
    
    public async Task<bool> PerformerExistsByEmail(string email, long projectId)
    {
        return await dbContext.ProjectPerformers
            .Include(e => e.Employee)
            .ThenInclude(e => e.User)
            .AnyAsync(e => e.Employee.User!.Email == email && e.ProjectId == projectId);
    }
    
}
