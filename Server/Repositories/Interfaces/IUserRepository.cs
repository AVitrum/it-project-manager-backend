using Server.Data.Models;
using Server.Data.SubModels;

namespace Server.Repositories.Interfaces;

public interface IUserRepository
{
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> DeleteAsync(User user);
    Task<User> GetAsync();
    Task<User> GetAsync(long id);
    Task<User> GetAsync(string username);
}