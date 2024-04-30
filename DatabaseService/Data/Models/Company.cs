using System.ComponentModel.DataAnnotations;

namespace DatabaseService.Data.Models;

public class Company
{
    [Key] public long Id { get; init; }
    public required string Name { get; set; }
    public required DateTime  RegistrationDate { get; set; }
    public string? Description { get; set; }
    public double? Budget { get; set; }

    public ICollection<PositionInCompany>? PositionInCompanies { get; set; }
    public ICollection<UserCompany>? UserCompanies { get; set; }
}

