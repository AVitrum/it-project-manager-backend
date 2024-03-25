using OAuth;
using UserHelper.Payload.Requests;

namespace Server.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(UserCreationRequest request);
    Task<string> LoginAsync(UserLoginRequest request);
    Task GoogleRegisterAsync(GoogleUserInfoResponse googleUserInfoResponse);
    Task<string> GoogleLoginAsync(string email);
    Task<bool> ExistsByEmail(string email);
    Task SendVerificationToken(string email);
    Task Verify(string token);
}