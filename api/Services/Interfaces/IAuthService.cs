using api.Data.Requests;

namespace api.Services.Interfaces;

public interface IAuthService
{
    void Register(UserCreationRequest request);
    string Login(UserLoginRequest request);
}