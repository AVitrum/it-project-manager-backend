using DatabaseService.Data.Models;
using Task = System.Threading.Tasks.Task;

namespace DatabaseService.Repositories.Interfaces;

public interface IAssignmentRepository
{
    Task CreateAsync(Assignment assignment);
    Task UpdateAsync(Assignment assignment);
    Task<bool> DeleteAsync(Assignment assignment);
    Task<Assignment> GetByIdAsync(long assignmentId);
    Task<Assignment> GetByThemeAsync(string theme);

    Task AddPerformer(AssignmentPerformer performer);

    Task<bool> PerformerExistsByEmail(string email);
}
