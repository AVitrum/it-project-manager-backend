using System.ComponentModel.DataAnnotations;
using Server.Data.SubModels;

namespace Server.Data.Models;

public class Team
{
    [Key]
    public long Id { get; set; }

    public string Name { get; set; }
    
    public ICollection<UserTeam> UserTeams { get; set; }
}

