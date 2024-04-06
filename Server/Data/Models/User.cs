using System.ComponentModel.DataAnnotations;

namespace Server.Data.Models;

public class User
{
    [Key] public long Id { get; init; }
    
    [MaxLength(50)]
    public required string Username { get; init; }

    [MaxLength(40)]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public required string Email { get; init; }
    [MaxLength(20)] public string? PhoneNumber { get; init; }
    public required DateTime RegistrationDate { get; init; }

    [MaxLength(64)] public string RefreshToken { get; set; } = string.Empty;
    public DateTime TokenCreated { get; set; }
    public DateTime TokenExpires { get; set; }
    
    public required byte[] PasswordHash { get; set; } = new byte[64];
    public required byte[] PasswordSalt { get; set; } = new byte[128];

    [MaxLength(64)] public string? VerificationToken { get; init; }
    public DateTime? VerifiedAt { get; set; }

    [MaxLength(64)] public string? PasswordResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }
    
    public ICollection<UserCompany> UserCompanies { get; init; }
}