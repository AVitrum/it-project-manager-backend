using OAuthService;
using OAuthService.Payload;
using Server.Payload.Requests;
using Server.Payload.Responses;

namespace Server.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(UserCreationRequest request);
    Task<LoginResponse> LoginAsync(UserLoginRequest request);
    Task<bool> GoogleRegisterAsync(GoogleUserInfoResponse googleUserInfoResponse);
    Task<LoginResponse> GoogleLoginAsync(string email);
    Task<bool> ExistsByEmail(string email);
    Task SendVerificationToken(string email);
    Task VerifyAsync(string token);
    Task<LoginResponse> RefreshAsync(RefreshRequest request);
}