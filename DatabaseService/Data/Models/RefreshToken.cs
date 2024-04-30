using System.ComponentModel.DataAnnotations;

namespace DatabaseService.Data.Models;

public class RefreshToken
{
    [Key]public long Id { get; init; }
    [MaxLength(128)] public required string Token { get; set; }
    public required DateTime Created { get; set; }
    public required DateTime Expires { get; set; }
    public required bool Expired { get; set; }

    public long UserId { get; set; }
    public User User { get; set; } = null!;
}