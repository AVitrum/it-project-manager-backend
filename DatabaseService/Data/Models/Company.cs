using System.ComponentModel.DataAnnotations;

namespace DatabaseService.Data.Models;

public class Company
{
    [Key] public long Id { get; init; }
    [MaxLength(40)] public required string Name { get; set; }
    public required DateTime  RegistrationDate { get; init; }
    [MaxLength(1200)] public string Description { get; set; } = string.Empty;
    public double Budget { get; set; }
    public double RemainingBudget { get; set; }
    
    public string? PictureName { get; set; }
    public string? PictureLink { get; set; }

    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<PositionInCompany>? PositionInCompanies { get; set; }
    public ICollection<User>? Users { get; set; }
}

