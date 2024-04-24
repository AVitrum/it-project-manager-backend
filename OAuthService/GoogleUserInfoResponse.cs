using System.Text.Json.Serialization;

namespace OAuthService;

public class GoogleUserInfoResponse
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("sub")]
    public required string Sub { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("given_name")]
    public required string GivenName { get; init; }

    [JsonPropertyName("family_name")]
    public required string FamilyName { get; init; }

    [JsonPropertyName("picture")]
    public required string Picture { get; init; }
}