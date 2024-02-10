using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Server.Data.SubModels;

namespace Server.Data.Models;

public class User
{
    [Key]
    public long Id { get; set; }
    
    [MaxLength(20)]
    public required string Username { get; set; }
    
    [MaxLength(40)]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public required string Email { get; set; }
    
    public required string PasswordHash { get; set; }
    
    public string? Image { get; set; }
    
    [NotMapped] 
    public IFormFile? ImageFile { get; set; }
    
    public ICollection<AdditionalUserInfo> AdditionalInfo { get; set; }
    
    public ICollection<UserTeam> UserTeams { get; set; }
}