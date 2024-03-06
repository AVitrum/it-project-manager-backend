using System.Text.Json;

namespace OAuth;

public static class GoogleProfileService
{
    public static async Task<UserInfoResponse?> GetUserInfoAsync(string accessToken)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var userInfo = JsonSerializer.Deserialize<UserInfoResponse>(content);

        return userInfo;
    }
}