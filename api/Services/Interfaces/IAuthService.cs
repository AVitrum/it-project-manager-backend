using api.Data.Models;

namespace api.Services.Interfaces;

public interface IAuthService
{
    void CreateUser(User user);
    string CreateToken(User user);
}