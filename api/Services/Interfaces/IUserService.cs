using api.Data.Models;

namespace api.Services.Interfaces;

public interface IUserService
{
    void CreateUser(User user);
    User GetUserByUsername(string username);
    string GetName();
}