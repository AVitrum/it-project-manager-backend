using Server.Data.Requests;
using UserHelper;
using UserHelper.Payload.Requests;

namespace Server.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(UserCreationRequest request);
    Task<string> LoginAsync(UserLoginRequest request);
    Task SendVerificationToken(string email);
    Task Verify(string token);
}