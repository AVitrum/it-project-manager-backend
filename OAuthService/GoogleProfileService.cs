using System.Net.Http.Headers;
using System.Text.Json;

namespace OAuthService;

public static class GoogleProfileService
{
    public static async Task<GoogleUserInfoResponse> GetUserProfileAsync(string accessToken)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var userInfo = JsonSerializer.Deserialize<GoogleUserInfoResponse>(content);

        if (userInfo == null)
            throw new GoogleOAuthException("Google OAuth Error");
        
        return userInfo;
    }
}