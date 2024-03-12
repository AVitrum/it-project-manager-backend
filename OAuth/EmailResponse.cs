using System.Text.Json.Serialization;

namespace OAuth;

public class EmailResponse
{
    [JsonPropertyName("email")]
    public string? Email { get; init; }
}