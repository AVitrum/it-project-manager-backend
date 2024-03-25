using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Server.Config;
using Server.Data.Models;
using Server.Exceptions;
using Server.Repositories.Interfaces;

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

    public async Task<bool> DeleteAsync(User user)
    {
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<User> GetByCurrentTokenAsync()
    {
        return httpContextAccessor.HttpContext is not null
            ? await GetByEmailAsync(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email)!)
            : throw new EntityNotFoundException(nameof(User));
    }

    public async Task<User> GetByIdAsync(long id)
    {
        return await dbContext.Users
//                   .Include(e => e.AdditionalInfo)
                   .FirstOrDefaultAsync(e => e.Id.Equals(id))
               ?? throw new EntityNotFoundException(nameof(User));
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await dbContext.Users
//                   .Include(u => u.AdditionalInfo)
                   .FirstOrDefaultAsync(u => u.Email.Equals(email))
               ?? throw new EntityNotFoundException(nameof(User));
    }

    public async Task<User> GetByTokenAsync(string token)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.VerificationToken!.Equals(token)) 
               ?? throw new EntityNotFoundException(nameof(User));
    }

    public async Task<User> GetByPasswordResetTokenAsync(string token)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.PasswordResetToken!.Equals(token)) 
               ?? throw new EntityNotFoundException(nameof(User)); 
    }

    public Task<bool> ExistsByEmailAsync(string email)
    {
        return dbContext.Users.AnyAsync(u => u.Email.Equals(email));
    }
}