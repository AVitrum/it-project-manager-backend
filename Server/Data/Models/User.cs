using System.ComponentModel.DataAnnotations;

namespace Server.Data.Models;

public class User
{
    [Key] public long Id { get; init; }
    
    [MaxLength(50)]
    public required string Username { get; init; }

    [MaxLength(64)]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public required string Email { get; init; }
    [MaxLength(20)] public string? PhoneNumber { get; init; }
    public required DateTime RegistrationDate { get; init; }
    
    public required byte[] PasswordHash { get; set; } = new byte[64];
    public required byte[] PasswordSalt { get; set; } = new byte[128];

    [MaxLength(128)] public string? VerificationToken { get; init; }
    public DateTime? VerifiedAt { get; set; }

    [MaxLength(128)] public string? PasswordResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; } = new List<RefreshToken>();
    public ICollection<UserCompany> UserCompanies { get; init; }
}