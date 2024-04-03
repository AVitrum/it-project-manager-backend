using System.Text.Json.Serialization;

namespace OAuthService.Payload;

public class EmailResponse
{
    [JsonPropertyName("email")]
    public string? Email { get; init; }
}