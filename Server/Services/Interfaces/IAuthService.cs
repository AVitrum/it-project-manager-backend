using OAuthService;
using Server.Payload.Requests;
using Server.Payload.Responses;

namespace Server.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegistrationRequest request);
    Task<LoginResponse> LoginAsync(UserLoginRequest request);
    Task<bool> GoogleOAuthRegistrationAsync(GoogleUserInfoResponse googleUserInfoResponse);
    Task<LoginResponse> GoogleOAuthLoginAsync(string email);
    Task<bool> ExistsByEmail(string email);
    Task SendVerificationToken(string email);
    Task VerifyAccountAsync(string token);
    Task<LoginResponse> RefreshJwtAsync(RefreshRequest request);
}