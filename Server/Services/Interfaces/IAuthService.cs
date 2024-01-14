using Server.Data.Requests;

namespace Server.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(UserCreationRequest request);
    Task<string> LoginAsync(UserLoginRequest request);
}