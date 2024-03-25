using Server.Data.Enums;

namespace Server.Data.Models;

public class UserCompany
{
    public long UserId { get; set; }
    public User User { get; set; }

    public long CompanyId { get; set; }
    public Company Company { get; set; }
    public UserRole Role { get; set; }
}