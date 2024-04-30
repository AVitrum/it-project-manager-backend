using DatabaseService.Data.Models;

namespace Server.Repositories.Interfaces;

public interface IUserRepository
{
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> DeleteAsync(User user);
    
    Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
    
    Task AddProfilePhoto(ProfilePhoto profilePhoto);
    Task<bool> DeleteProfilePhotoAsync(ProfilePhoto profilePhoto);

    Task<User> GetByIdAsync(long id);
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByJwtAsync();
    Task<(User, ProfilePhoto?)> GetByJwtWithPhotoAsync();
    Task<User> GetByTokenAsync(string token);
    Task<(User, RefreshToken)> GetByRefreshToken(string refreshToken);
    Task<User> GetByPasswordResetTokenAsync(string token);
    
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
}