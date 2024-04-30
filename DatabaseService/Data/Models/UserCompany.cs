namespace DatabaseService.Data.Models;

public class UserCompany
{
    public required long UserId { get; set; }
    public User? User { get; set; }
    public required long CompanyId { get; set; }
    public Company? Company { get; set; }
    public required long PositionInCompanyId { get; set; }
    public PositionInCompany? PositionInCompany { get; set; }
    
    public double Salary { get; set; } = 0;
}