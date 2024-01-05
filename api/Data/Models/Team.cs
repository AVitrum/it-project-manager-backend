using System.ComponentModel.DataAnnotations;
using api.Data.SubModels;

namespace api.Data.Models;

public class Team
{
    [Required, Key]
    public long Id { get; set; }

    public string Name { get; set; }
    
    public ICollection<UserTeam> UserTeams { get; set; }
}

