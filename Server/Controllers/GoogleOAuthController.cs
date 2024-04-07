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
            "https://www.googleapis.com/auth/userinfo.email",
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

        var email = await GoogleProfileService.GetUserEmailAsync(tokenResult!.AccessToken);

        if (await authService.ExistsByEmail(email!))
        {
            var token = await authService.GoogleLoginAsync(email!);
            return Redirect($"{configuration.GetSection("AppSettings:FrontendUrl").Value}/{token}");
        }

        HttpContext.Session.SetString("email", email!);

        var url = GoogleOAuthService
            .GenerateOAuthRequestUrl(
                "https://www.googleapis.com/auth/userinfo.profile",
                "https://localhost:8080/GoogleOAuth/Profile",
                CodeChallenge);
        return Redirect(url);
    }
    public async Task<IActionResult> ProfileAsync(string? code)
    {
        var codeVerifier = HttpContext.Session.GetString(PkceSessionKey);
        var tokenResult = await GoogleOAuthService
            .ExchangeCodeOnTokenAsync(
                code,
                codeVerifier,
                "https://localhost:8080/GoogleOAuth/Profile");

        var profile = await GoogleProfileService.GetUserProfileAsync(tokenResult!.AccessToken);
        // var refreshedTokenResult = await GoogleOAuthService.RefreshTokenAsync(tokenResult.RefreshToken);
        var email = HttpContext.Session.GetString("email");
        profile!.Email = email;

        await authService.GoogleRegisterAsync(profile);

        var token = await authService.GoogleLoginAsync(email!);
        return Redirect($"{configuration.GetSection("AppSettings:FrontendUrl")}" +
                        $"/AccessToken:{token.AccessToken}RefreshToken:{token.RefreshToken}");
    }
}