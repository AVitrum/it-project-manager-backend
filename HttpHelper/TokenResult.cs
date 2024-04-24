using Newtonsoft.Json;

namespace HttpHelper;

public class TokenResult
{
    [JsonProperty("access_token")]
    public required string AccessToken { get; set; }

    [JsonProperty("expires_in")]
    public required string ExpiresIn { get; set; }

    [JsonProperty("scope")]
    public required string Scope { get; set; }

    [JsonProperty("token_type")]
    public required string TokenType { get; set; }

    [JsonProperty("refresh_token")]
    public required string RefreshToken { get; set; }
}