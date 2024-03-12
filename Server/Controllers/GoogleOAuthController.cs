using Microsoft.AspNetCore.Mvc;
using OAuth;
using Server.Services.Interfaces;

namespace Server.Controllers;

public class GoogleOAuthController(IAuthService authService) : Controller
{
    private const string PkceSessionKey = "codeVerifier";

    public IActionResult RedirectOnOAuthServer()
    {
        var codeVerifier = Guid.NewGuid().ToString();
        var codeChallenge = Sha256Helper.ComputeHash(codeVerifier);

        HttpContext.Session.SetString(PkceSessionKey, codeVerifier);

        var url = GoogleOAuthService.GenerateOAuthRequestUrl(
            "https://www.googleapis.com/auth/userinfo.email",
            "https://localhost:8080/GoogleOAuth/Login",
            codeChallenge);
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
        // var refreshedTokenResult = await GoogleOAuthService.RefreshTokenAsync(tokenResult.RefreshToken);
        var token = await authService.GoogleLoginAsync(email!);
        return Ok(token);
    }

    public IActionResult Register()
    {
        var codeVerifier = Guid.NewGuid().ToString();
        var codeChallenge = Sha256Helper.ComputeHash(codeVerifier);

        HttpContext.Session.SetString(PkceSessionKey, codeVerifier);
        var url = GoogleOAuthService
            .GenerateOAuthRequestUrl(
                "https://www.googleapis.com/auth/userinfo.profile",
                "https://localhost:8080/GoogleOAuth/Profile",
                codeChallenge);
        return Redirect(url);
    }
}