using System.ComponentModel.DataAnnotations;

namespace DatabaseService.Data.Models;

public class ProjectPerformer
{
    [Key] public long Id { get; init; }
    public required long EmployeeId { get; set; }
    public required Employee Employee { get; set; }
    public required long ProjectId { get; set; }
    public required Project Project { get; set; }
}
