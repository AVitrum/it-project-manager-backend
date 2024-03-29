using Server.Data.Enums;

namespace Server.Data.Models;

public class UserCompany
{
    public required long UserId { get; set; }
    public required long CompanyId { get; set; }
    public required EmployeeRole Role { get; set; } // Replace with the ability to create your own roles with their own rights
    public User? User { get; set; }
    public Company? Company { get; set; }
    public double? Salary { get; set; }
}