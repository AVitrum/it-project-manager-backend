using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

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
            .ThenInclude(e => e.User)
            .Include(e => e.Company)
            .FirstOrDefaultAsync(e => e.Id == projectId) 
               ?? throw new EntityNotFoundException(nameof(Project));
    }

    public async Task<Project> GetByIdSql(long projectId)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            """
            
                        SELECT p.*, a."Id" AS "AssignmentId", a."Theme", a."Description" AS "TaskDescription", a."CreatedAt", a."Deadline" 
                        FROM "Projects" AS p
                        LEFT JOIN "Assignments" AS a ON a."ProjectId" = p."Id"
                        WHERE p."Id" = @id
                        ORDER BY "Id" 
                        
            """,
            connection);
        command.Parameters.AddWithValue("id", projectId);

        await using var reader = await command.ExecuteReaderAsync();

        Project? project = null;

        while (await reader.ReadAsync())
        {
            project ??= new Project
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) 
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("Description")),
                Budget = reader.GetDouble(reader.GetOrdinal("Budget")),
                CreatorId = reader.GetInt64(reader.GetOrdinal("CreatorId")),
                CompanyId = reader.GetInt64(reader.GetOrdinal("CompanyId")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                PictureLink = reader.IsDBNull(reader.GetOrdinal("PictureLink")) 
                    ? null
                    : reader.GetString(reader.GetOrdinal("PictureLink")),
                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate"))
            };
            
            if (reader.IsDBNull(reader.GetOrdinal("AssignmentId"))) continue;
            var assignment = new Assignment
            {
                Id = reader.GetInt64(reader.GetOrdinal("AssignmentId")),
                Description = reader.IsDBNull(reader.GetOrdinal("TaskDescription")) 
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("TaskDescription")),
                ProjectId = projectId,
                Theme = reader.GetString(reader.GetOrdinal("Theme")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                Budget = reader.GetDouble(reader.GetOrdinal("Budget")),
                Deadline = reader.GetDateTime(reader.GetOrdinal("Deadline"))
            };
            
            project.Assignments.Add(assignment);
        }

        return project ?? throw new EntityNotFoundException(nameof(Project));
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
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            """
            
                        SELECT p."Id", p."CompanyId" 
                        FROM "Projects" AS p
                        WHERE p."CompanyId" = @id
                        ORDER BY "Id" 
                        
            """,
            connection);
        command.Parameters.AddWithValue("id", companyId);

        await using var reader = await command.ExecuteReaderAsync();

        var projects = new List<Project>();

        while (await reader.ReadAsync())
        {
            var id = reader.GetInt64(reader.GetOrdinal("Id"));
            var project = await GetByIdSql(id);
            
            projects.Add(project);
        }

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
    
    public async Task<bool> PerformerExistsByEmail(string email)
    {
        return await dbContext.ProjectPerformers
            .Include(e => e.Employee)
            .ThenInclude(e => e.User)
            .AnyAsync(e => e.Employee.User!.Email == email);
    }
    
}
