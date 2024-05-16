using System.ComponentModel.DataAnnotations;

namespace DatabaseService.Data.Models;

public class Assignment
{
    [Key] public long Id { get; init; }
    
    public required long ProjectId { get; set; }
    public Project? Project { get; set; }
    
    [MaxLength(60)]
    public required string Theme { get; set; }
    [MaxLength(1200)]
    public string? Description { get; set; } = string.Empty;
    public double Budget { get; set; } = 0; 
    public required DateTime CreatedAt { get; set; }
    public required DateTime Deadline { get; set; }
    
    public ICollection<AssignmentPerformer> Performers { get; set; } = new List<AssignmentPerformer>();
}