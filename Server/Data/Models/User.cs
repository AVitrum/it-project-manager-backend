using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    public required byte[] PasswordHash { get; set; } = new byte[32];

    public required byte[] PasswordSalt { get; set; } = new byte[32];
    
    public string? VerificationToken { get; set; }
    
    public DateTime? VerifiedAt { get; set; }

    public string? PasswordResetToken { get; set; }
    
    public DateTime? ResetTokenExpires { get; set; }
    
    public required DateTime RegistrationDate { get; set; }
    
    public ICollection<UserTeam> UserTeams { get; set; }
}