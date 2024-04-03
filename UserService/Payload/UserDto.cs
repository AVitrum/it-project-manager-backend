namespace UserService.Payload;

public class UserDto
{
    public string? Username { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public DateTime? VerifiedAt { get; set; }
    public DateTime? RegistrationDate { get; set; }
}