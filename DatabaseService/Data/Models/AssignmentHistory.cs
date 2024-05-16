using System.ComponentModel.DataAnnotations;

namespace DatabaseService.Data.Models;

public class AssignmentHistory
{
    [Key] public long Id { get; init; }
    
    public required long AssignmentId { get; init; }
    public Assignment? Assignment { get; set; }
    
    public required DateTime UpdatedAt { get; init; }

    [MaxLength(1024)] public string Change { get; set; } = string.Empty;
}