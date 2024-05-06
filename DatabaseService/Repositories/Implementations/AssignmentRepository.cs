using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatabaseService.Repositories.Implementations;

public class AssignmentRepository(AppDbContext dbContext) : IAssignmentRepository
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