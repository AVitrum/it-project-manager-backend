namespace UserService.Payload.Requests;

public class TokenRequest
{
    public string? Email { get; set; }
    public string? Token { get; set; }
}