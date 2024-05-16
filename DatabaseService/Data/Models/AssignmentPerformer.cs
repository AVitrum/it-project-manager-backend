using System.ComponentModel.DataAnnotations;

namespace DatabaseService.Data.Models;

public class AssignmentPerformer
{
    [Key] public long Id { get; init; }
    
    public required long ProjectPerformerId { get; set; }
    public required ProjectPerformer ProjectPerformer { get; set; }
    public required long AssignmentId { get; set; }
    public required Assignment Assignment { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}