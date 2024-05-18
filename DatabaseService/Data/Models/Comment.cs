using System.ComponentModel.DataAnnotations;

namespace DatabaseService.Data.Models;

public class Comment
{
    [Key] public long Id { get; init; }
    
    public required long UserId { get; set; }
    public AssignmentPerformer User { get; set; }
    
    public required long AssignmentId { get; set; }
    public Assignment Assignment { get; set; }
    
    public required string Message { get; set; }
    public required DateTime CreatedAt { get; set; }
}