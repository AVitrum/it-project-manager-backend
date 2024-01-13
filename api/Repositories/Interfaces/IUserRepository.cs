using api.Data.Models;
using api.Data.SubModels;

namespace api.Repositories.Interfaces;

public interface IUserRepository
{
    void Create(User user);
    void Update(User user);
    bool Delete(User user);

    User GetById(long id);
    User GetByUsername(string username);
    User GetFromToken();


    void SaveAdditionalInfo(AdditionalUserInfo additionalUserInfo);
}