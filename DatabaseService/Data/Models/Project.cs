using System.ComponentModel.DataAnnotations;

namespace DatabaseService.Data.Models;

public class Project
{
    [Key] public long Id { get; init; }

    public required long CreatorId { get; set; }
    public Employee? Creator { get; set; }
    public required long CompanyId { get; set; }
    public Company? Company { get; set; }

    [MaxLength(40)]
    public required string Name { get; set; }

    [MaxLength(1200)] public string Description { get; set; } = string.Empty;
    public double Budget { get; set; } = 0;
    public required DateTime CreationDate { get; set; }

    public ICollection<ProjectPerformer>? ProjectPerformers { get; set; }
}