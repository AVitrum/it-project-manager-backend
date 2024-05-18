using System.ComponentModel.DataAnnotations;

namespace DatabaseService.Data.Models;

public class AssignmentFile
{
    [Key] public long Id { get; init; }
    
    public required long AssignmentId { get; init; }
    public Assignment Assignment { get; init; }
    
    public required string Name { get; set; }
    public required string DbName { get; set; }
    public required string Link { get; set; }
}