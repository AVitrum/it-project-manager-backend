using System.ComponentModel.DataAnnotations;

namespace DatabaseService.Data.Models;

public class User
{
    [Key] public long Id { get; init; }

    [MaxLength(50)]
    public required string Username { get; set; }

    [MaxLength(64)]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public required string Email { get; init; }
    [MaxLength(20)] public string? PhoneNumber { get; set; }
    public required DateTime RegistrationDate { get; init; }
    public required byte[] PasswordHash { get; set; } = new byte[64];
    public required byte[] PasswordSalt { get; set; } = new byte[128];
    [MaxLength(128)] public string? VerificationToken { get; init; }
    public DateTime? VerifiedAt { get; set; }
    [MaxLength(128)] public string? PasswordResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }


    public ProfilePhoto? ProfilePhoto { get; set; }
    public IEnumerable<RefreshToken> RefreshTokens { get; } = new List<RefreshToken>();
    public IEnumerable<Company> Companies { get; } = new List<Company>();
}