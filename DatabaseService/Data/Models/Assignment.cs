using System.ComponentModel.DataAnnotations;
using DatabaseService.Data.Enums;

namespace DatabaseService.Data.Models;

public class Assignment
{
    [Key] public long Id { get; init; }
    
    public required long ProjectId { get; init; }
    public Project? Project { get; set; }
    
    [MaxLength(60)]
    public required string Theme { get; set; }
    [MaxLength(1200)]
    public string? Description { get; set; } = string.Empty;
    public double Budget { get; set; } = 0; 
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdateAt { get; set; }
    public required DateTime Deadline { get; set; }

    [EnumDataType(typeof(AssignmentType))]
    public AssignmentType Type { get; set; } = AssignmentType.ASSIGNED;
    
    public ICollection<AssignmentPerformer> Performers { get; set; } = new List<AssignmentPerformer>();
    public ICollection<AssignmentHistory> Changes { get; set; } = new List<AssignmentHistory>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<AssignmentFile> Files { get; set; } = new List<AssignmentFile>();
}