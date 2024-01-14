using api.Data.Requests;

namespace api.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(UserCreationRequest request);
    Task<string> LoginAsync(UserLoginRequest request);
}