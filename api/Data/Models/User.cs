using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using api.Data.SubModels;

namespace api.Data.Models;

public class User
{

    [Key]
    public long Id { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Username { get; set; }
    
    [Required]
    [MaxLength(40)]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }
    
    public string PasswordHash { get; set; }
    
    public string? Image { get; set; }
    
    [NotMapped] 
    public IFormFile? ImageFile { get; set; }
    
    public ICollection<AdditionalUserInfo> AdditionalInfo { get; set; }
    
    public ICollection<UserTeam> UserTeams { get; set; }
}