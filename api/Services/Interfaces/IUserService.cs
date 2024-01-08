using api.Data.Models;
using api.Data.Responses;

namespace api.Services.Interfaces;

public interface IUserService
{
    void CreateUser(User user);
    User GetById(long id);
    User GetByUsername(string username);
    User GetFromToken();
}