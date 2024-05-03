namespace DatabaseService.Data.Models;

public class UserCompany
{
    public required long UserId { get; init; }
    public User? User { get; init; }
    public required long CompanyId { get; init; }
    public Company? Company { get; init; }
    public required long PositionInCompanyId { get; init; }
    public PositionInCompany? PositionInCompany { get; set; }
    
    public double Salary { get; set; }
}