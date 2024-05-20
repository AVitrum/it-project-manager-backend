using Microsoft.AspNetCore.Mvc;
using OAuthService;
using Server.Services.Interfaces;

namespace Server.Controllers;

public class GoogleOAuthController(IConfiguration configuration, IAuthService authService) : Controller
{
    private const string PkceSessionKey = "codeVerifier";
    private static readonly string CodeVerifier = Guid.NewGuid().ToString();
    private static readonly string? CodeChallenge = Sha256Helper.ComputeHash(CodeVerifier);

    public IActionResult RedirectOnOAuthServer()
    {
        HttpContext.Session.SetString(PkceSessionKey, CodeVerifier);

        var url = GoogleOAuthService.GenerateOAuthRequestUrl(
            "https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/calendar https://www.googleapis.com/auth/calendar.events",
            "https://localhost:8080/GoogleOAuth/Login",
            CodeChallenge);
        return Redirect(url);
    }

    public async Task<IActionResult> LoginAsync(string? code)
    {
        var codeVerifier = HttpContext.Session.GetString(PkceSessionKey);
        var tokenResult = await GoogleOAuthService
            .ExchangeCodeOnTokenAsync(
                code,
                codeVerifier,
                "https://localhost:8080/GoogleOAuth/Login");

        var response = await GoogleProfileService.GetUserProfileAsync(tokenResult.AccessToken);

        if (await authService.ExistsByEmail(response.Email))
        {
            var tokens = await authService.GoogleOAuthLoginAsync(response.Email, tokenResult.AccessToken);
            return Redirect($"{configuration.GetSection("AppSettings:FrontendUrl").Value}" + "/OAuth" +
                            $"/?AccessToken={tokens.AccessToken}&RefreshToken={tokens.RefreshToken}");
        }
        else
        {
            await authService.GoogleOAuthRegistrationAsync(response);

            var tokens = await authService.GoogleOAuthLoginAsync(response.Email, tokenResult.AccessToken);
            return Redirect($"{configuration.GetSection("AppSettings:FrontendUrl").Value}" + "/OAuth" +
                            $"/?AccessToken={tokens.AccessToken}&RefreshToken={tokens.RefreshToken}");
        }
    }
}