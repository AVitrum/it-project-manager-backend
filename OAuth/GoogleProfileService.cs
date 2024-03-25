using System.Net.Http.Headers;
using System.Text.Json;

namespace OAuth;

public static class GoogleProfileService
{
    public static async Task<GoogleUserInfoResponse?> GetUserProfileAsync(string accessToken)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var userInfo = JsonSerializer.Deserialize<GoogleUserInfoResponse>(content);
        
        return userInfo;
    }
    
    public static async Task<string?> GetUserEmailAsync(string accessToken)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var emailResponse = await client.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
        emailResponse.EnsureSuccessStatusCode();

        var emailContent = await emailResponse.Content.ReadAsStringAsync();
        var emailInfo = JsonSerializer.Deserialize<EmailResponse>(emailContent);

        return emailInfo!.Email;
    }
}