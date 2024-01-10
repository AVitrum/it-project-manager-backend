using api.Data.Models;
using api.Data.Requests;
using api.Data.Responses;
using api.Data.SubModels;

namespace api.Services.Interfaces;

public interface IUserService
{
    void CreateUser(User user);
    void AddInfo(AdditionalUserInfo additionalUserInfo);
    User GetById(long id);
    User GetByUsername(string username);
    User GetFromToken();
}