namespace UserHelper;

public class RefreshTokenDto
{
    public required string Token { get; set; }
    public required bool Expired { get; set; }
    public required DateTime Created { get; set; } = DateTime.UtcNow;
    public required DateTime Expires { get; set; }
}