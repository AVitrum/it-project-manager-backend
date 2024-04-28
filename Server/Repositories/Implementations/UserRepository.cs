using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Server.Config;
using Server.Data.Models;
using Server.Exceptions;
using Server.Repositories.Interfaces;
using UserHelper;

namespace Server.Repositories.Implementations;

public class UserRepository(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext) : IUserRepository
{
    public async Task CreateAsync(User user)
    {
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
    {
        if (await dbContext.RefreshTokens.AnyAsync(e => e.Token == refreshToken.Token))
            dbContext.RefreshTokens.Update(refreshToken);
        else
            await dbContext.RefreshTokens.AddAsync(refreshToken);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(User user)
    {
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<User> GetByCurrentTokenAsync()
    {
        const string email = ClaimTypes.Email;

        return httpContextAccessor.HttpContext is not null
            ? await GetByEmailAsync(httpContextAccessor.HttpContext.User.FindFirstValue(email)!)
            : throw new UserNotFoundException(email);
    }

    public async Task<(User, RefreshToken)> GetByRefreshToken(string refreshToken)
    {
        var query = 
            from user in dbContext.Users 
            join token in dbContext.RefreshTokens on user.Id equals token.UserId 
            where token.Token == refreshToken 
            select new { user, token };

        var result = await query.FirstOrDefaultAsync() ?? throw new EntityNotFoundException(nameof(User));
        return (result.user, result.token);
    }


    public async Task<User> GetByIdAsync(long id)
    {
        return await dbContext.Users
                   .FirstOrDefaultAsync(e => e.Id.Equals(id))
               ?? throw new UserNotFoundException(id);
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await dbContext.Users
                   .FirstOrDefaultAsync(u => u.Email.Equals(email))
               ?? throw new UserNotFoundException(email);
    }

    public async Task<User> GetByTokenAsync(string token)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.VerificationToken!.Equals(token))
               ?? throw new UserException("Wrong Verification Token!");
    }

    public async Task<User> GetByPasswordResetTokenAsync(string token)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.PasswordResetToken!.Equals(token))
               ?? throw new UserException("Wrong Password Reset Token!");
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await dbContext.Users.AnyAsync(u => u.Email.Equals(email));
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await dbContext.Users.AnyAsync(u => u.Username.Equals(username));
    }
}