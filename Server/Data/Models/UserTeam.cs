using Server.Data.Enums;

namespace Server.Data.Models;

public class UserTeam
{
    public long UserId { get; set; }
    public User User { get; set; }

    public long TeamId { get; set; }
    public Team Team { get; set; }
    public UserRole Role { get; set; }
}