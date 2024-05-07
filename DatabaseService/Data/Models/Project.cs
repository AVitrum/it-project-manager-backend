using System.ComponentModel.DataAnnotations;

namespace DatabaseService.Data.Models;

public class Project
{
    [Key] public long Id { get; init; }

    public required long CreatorId { get; set; }
    public required Employee Creator { get; set; }
    public required long CompanyId { get; set; }
    public required Company Company { get; set; }

    [MaxLength(40)]
    public required string Name { get; set; }
    [MaxLength(1200)] public string Description { get; set; } = string.Empty;
    public double Budget { get; set; }
    
    public string? PictureName { get; set; }
    public string? PictureLink { get; set; }
    
    public required DateTime CreationDate { get; set; }

    public ICollection<ProjectPerformer>? ProjectPerformers { get; set; }
}