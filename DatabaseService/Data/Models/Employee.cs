using System.ComponentModel.DataAnnotations;

namespace DatabaseService.Data.Models;

public class Employee
{
    [Key] public long Id { get; init; }
    public required long UserId { get; set; }
    public User? User { get; init; }
    public required long CompanyId { get; set; }
    public Company? Company { get; init; }
    public required long PositionInCompanyId { get; init; }
    public required PositionInCompany PositionInCompany { get; set; }
    
    public double Salary { get; set; }

    public ICollection<ProjectPerformer>? ProjectPerformers{ get; set; }
}