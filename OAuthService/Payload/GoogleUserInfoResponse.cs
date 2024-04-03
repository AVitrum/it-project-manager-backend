using System.Text.Json.Serialization;

namespace OAuthService.Payload;

public class GoogleUserInfoResponse
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("sub")]
    public string? Sub { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("given_name")]
    public string? GivenName { get; init; }

    [JsonPropertyName("family_name")]
    public string? FamilyName { get; init; }

    [JsonPropertyName("picture")]
    public string? Picture { get; init; }

    [JsonPropertyName("locale")]
    public string? Locale { get; init; }
}