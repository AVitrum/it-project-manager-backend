using DatabaseService.Data.Models;
using Task = System.Threading.Tasks.Task;

namespace DatabaseService.Repositories.Interfaces;

public interface IAssignmentRepository
{
    Task<Assignment> CreateAsync(Assignment assignment);
    Task UpdateAsync(Assignment assignment);
    Task<bool> DeleteAsync(Assignment assignment);
    Task<Assignment> GetByIdAsync(long assignmentId);
    Task<Assignment> GetByThemeAndProjectIdAsync(string theme, long projectId);
    Task<List<Assignment>> GetAllByProjectIdAsync(long projectId);
    
    Task AddPerformer(AssignmentPerformer performer);
    Task<AssignmentPerformer> GetPerformerByProjectPerformerAndAssignment(ProjectPerformer projectPerformer, Assignment assignment);

    Task AddComment(Comment comment);
    
    Task AddChange(AssignmentHistory change);
    Task<List<AssignmentHistory>> GetChanges(Assignment assignment);
    
    Task<bool> PerformerExistsByEmail(string email);
}
