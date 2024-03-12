using System.Text.Json.Serialization;

namespace OAuth;

public class GoogleUserInfoResponse
{
    [JsonPropertyName("sub")]
    public string? Sub { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("given_name")]
    public string? GivenName { get; init; }

    [JsonPropertyName("family_name")]
    public string? FamilyName { get; init; }

    [JsonPropertyName("picture")]
    public string? Picture { get; init; }

    [JsonPropertyName("locale")]
    public string? Locale { get; init; }
}