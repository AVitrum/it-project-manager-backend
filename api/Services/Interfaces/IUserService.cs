using api.Data.Models;

namespace api.Services.Interfaces;

public interface IUserService
{
    void CreateUser(User user);
    User GetById(long id);
    User GetByUsername(string username);
    string GetName();
}