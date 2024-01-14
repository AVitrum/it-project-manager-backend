using System.Security.Claims;
using api.Config;
using api.Data.Models;
using api.Data.SubModels;
using api.Exceptions;
using api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories.Implementations;

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

    public async Task<User> GetAsync()
    {
        return httpContextAccessor.HttpContext is not null
            ? await GetAsync(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name)!)
            : throw new EntityNotFoundException(new User().GetType().Name);
    }

    public async Task<User> GetAsync(long id)
    {
        return await dbContext.Users
                   .Include(e => e.AdditionalInfo)
                   .FirstOrDefaultAsync(e => e.Id.Equals(id))
               ?? throw new EntityNotFoundException(new User().GetType().Name);
    }

    public async Task<User> GetAsync(string username)
    {
        return await dbContext.Users
                   .Include(u => u.AdditionalInfo)
                   .FirstOrDefaultAsync(u => u.Username.Equals(username))
               ?? throw new EntityNotFoundException(new User().GetType().Name);
    }

    public async Task SaveAdditionalInfoAsync(AdditionalUserInfo additionalUserInfo)
    {
        await dbContext.AdditionalUserInfos.AddAsync(additionalUserInfo);
        await dbContext.SaveChangesAsync();
    }
}