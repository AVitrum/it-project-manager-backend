using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using OAuth;
using Server.Services.Interfaces;

namespace Server.Controllers;

public class GoogleOAuthController(IAuthService authService) : Controller
{
    private const string? RedirectUrl = "https://localhost:8080/GoogleOAuth/Code";
    private const string? Scope = "https://www.googleapis.com/auth/userinfo.profile";
    private const string PkceSessionKey = "codeVerifier";

    public IActionResult RedirectOnOAuthServer()
    {
        var codeVerifier = Guid.NewGuid().ToString();
        var codeChallenge = Sha256Helper.ComputeHash(codeVerifier);

        HttpContext.Session.SetString(PkceSessionKey, codeVerifier);

        var url = GoogleOAuthService.GenerateOAuthRequestUrl(Scope, RedirectUrl, codeChallenge);
        return Redirect(url);
    }

    public async Task<IActionResult> CodeAsync(string? code)
    {
        var codeVerifier = HttpContext.Session.GetString(PkceSessionKey);
        var tokenResult = await GoogleOAuthService.ExchangeCodeOnTokenAsync(code, codeVerifier, RedirectUrl);
        
        var user = await GoogleProfileService.GetUserInfoAsync(tokenResult!.AccessToken);
        var refreshedTokenResult = await GoogleOAuthService.RefreshTokenAsync(tokenResult.RefreshToken);
        return Ok(user);
    }
}