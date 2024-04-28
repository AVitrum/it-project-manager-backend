using Server.Data.Models;

namespace Server.Repositories.Interfaces;

public interface IUserRepository
{
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task AddProfilePhoto(ProfilePhoto profilePhoto);
    Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
    Task<bool> DeleteAsync(User user);
    Task<bool> DeleteProfilePhotoAsync(ProfilePhoto profilePhoto);
    Task<User> GetByJwtAsync();
    Task<(User, ProfilePhoto?)> GetByJwtWithPhotoAsync();
    Task<(User, RefreshToken)> GetByRefreshToken(string refreshToken);
    Task<User> GetByIdAsync(long id);
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByTokenAsync(string token);
    Task<User> GetByPasswordResetTokenAsync(string token);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
}