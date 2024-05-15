using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatabaseService.Repositories.Implementations;

public class AssignmentRepository(AppDbContext dbContext) : IAssignmentRepository
{
    public async Task<Assignment> CreateAsync(Assignment assignment)
    {
        await dbContext.Assignments.AddAsync(assignment);
        await dbContext.SaveChangesAsync();

        return await GetByThemeAndProjectIdAsync(assignment.Theme, assignment.ProjectId);
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

    public async Task<Assignment> GetByIdAsync(long assignmentId)
    {
        return await dbContext.Assignments
                   .Include(a => a.Project)
                   .Include(a => a.Performers)
                   .ThenInclude(ap => ap.ProjectPerformer)
                   .ThenInclude(p => p.Employee)
                   .ThenInclude(e => e.User!.ProfilePhoto)
                   .Include(a => a.Performers)
                   .ThenInclude(ap => ap.ProjectPerformer)
                   .ThenInclude(p => p.Employee.PositionInCompany)
                   .FirstOrDefaultAsync(e => e.Id == assignmentId) 
               ?? throw new EntityNotFoundException(nameof(Assignment));
    }

    public async Task<List<Assignment>> GetAllByProjectIdAsync(long projectId)
    {
        return await dbContext.Assignments
            .Include(a => a.Project)
            .Include(a => a.Performers)
            .ThenInclude(ap => ap.ProjectPerformer)
            .ThenInclude(p => p.Employee)
            .ThenInclude(e => e.User!.ProfilePhoto)
            .Include(a => a.Performers)
            .ThenInclude(ap => ap.ProjectPerformer)
            .ThenInclude(p => p.Employee.PositionInCompany)
            .Where(a => a.ProjectId == projectId)
            .ToListAsync();
    }


    public async Task<Assignment> GetByThemeAndProjectIdAsync(string theme, long projectId)
    {
        return await dbContext.Assignments
                   .Include(a => a.Project)
                   .Include(a => a.Performers)
                   .ThenInclude(ap => ap.ProjectPerformer)
                   .ThenInclude(p => p.Employee)
                   .ThenInclude(e => e.User!.ProfilePhoto)
                   .Include(a => a.Performers)
                   .ThenInclude(ap => ap.ProjectPerformer)
                   .ThenInclude(p => p.Employee.PositionInCompany)
                   .FirstOrDefaultAsync(e => e.Theme == theme && e.ProjectId == projectId) 
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