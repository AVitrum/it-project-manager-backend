using Server.Data.Models;

namespace Server.Repositories.Interfaces;

public interface IUserRepository
{
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> DeleteAsync(User user);
    Task<User> GetAsync();
    Task<User> GetAsync(long id);
    Task<User> GetAsync(string username);
    Task<User> GetAsyncByToken(string token);
    Task<User> GetAsyncByPasswordResetToken(string token);
    Task<bool> ExistByEmailAsync(string email);
}