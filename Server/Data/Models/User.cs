using System.ComponentModel.DataAnnotations;

namespace Server.Data.Models;

public class User
{
    [Key]
    public long Id { get; set; }
    
    [MaxLength(50)]
    public required string Username { get; set; }
    
    [MaxLength(40)]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required DateTime RegistrationDate { get; set; }
    public required byte[] PasswordHash { get; set; } = new byte[64];
    public required byte[] PasswordSalt { get; set; } = new byte[128];
    public string? VerificationToken { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }
    
    public ICollection<UserCompany>? UserCompanies { get; set; }
}