using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DatabaseService.Repositories.Implementations;

public class AssignmentRepository(AppDbContext dbContext, IConfiguration configuration) : IAssignmentRepository
{
    public async Task CreateAsync(Assignment assignment)
    {
        await dbContext.Assignments.AddAsync(assignment);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Assignment assignment)
    {
        dbContext.Assignments.Update(assignment);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Assignment assignment)
    {
        dbContext.Assignments.Remove(assignment);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<Assignment>> GetAllByProjectIdAsync(long projectId)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            """
            
                        SELECT a.* 
                        FROM "Assignments" as a
                        WHERE a."ProjectId" = @id
                        ORDER BY a."Deadline" 
                        
            """,
            connection);
        command.Parameters.AddWithValue("id", projectId);

        await using var reader = await command.ExecuteReaderAsync();

        var assignments = new List<Assignment>();

        while (await reader.ReadAsync())
        {
            var assignment = new Assignment
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) 
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("Description")),
                ProjectId = projectId,
                Theme = reader.GetString(reader.GetOrdinal("Theme")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                Budget = reader.GetDouble(reader.GetOrdinal("Budget")),
                Deadline = reader.GetDateTime(reader.GetOrdinal("Deadline"))
            };
            
            assignments.Add(assignment);
        }

        return assignments;
    } 

    public async Task<Assignment> GetByIdAsync(long assignmentId)
    {
        return await dbContext.Assignments
            .Include(e => e.Project)
            .ThenInclude(e => e.Company)
            .Include(e => e.Project)
            .ThenInclude(e => e.Creator)
            .Include(e => e.Project)
            .ThenInclude(e => e.ProjectPerformers)
            .Include(e => e.Performers)
            .ThenInclude(e => e.ProjectPerformer)
            .ThenInclude(e => e.Employee)
            .ThenInclude(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == assignmentId) 
               ?? throw new EntityNotFoundException(nameof(Assignment));
    }

    public async Task<Assignment> GetByThemeAsync(string theme)
    {
        return await dbContext.Assignments
                   .Include(e => e.Project)
                   .ThenInclude(e => e.Company)
                   .Include(e => e.Project)
                   .ThenInclude(e => e.Creator)
                   .Include(e => e.Project)
                   .ThenInclude(e => e.ProjectPerformers)
                   .Include(e => e.Performers)
                   .ThenInclude(e => e.ProjectPerformer)
                   .ThenInclude(e => e.Employee)
                   .ThenInclude(e => e.User)
                   .FirstOrDefaultAsync(e => e.Theme == theme) 
               ?? throw new EntityNotFoundException(nameof(Assignment)); 
    }

    public async Task AddPerformer(AssignmentPerformer performer)
    {
        await dbContext.AssignmentPerformers.AddAsync(performer);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> PerformerExistsByEmail(string email)
    {
        return await dbContext.AssignmentPerformers
            .Include(e => e.ProjectPerformer)
            .ThenInclude(e => e.Employee)
            .ThenInclude(e => e!.User)
            .AnyAsync(e => e.ProjectPerformer.Employee!.User!.Email == email);
    }
}