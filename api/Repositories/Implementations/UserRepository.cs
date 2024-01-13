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
    public void Create(User user)
    {
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
    }
    
    public void Update(User user)
    {
        dbContext.Users.Update(user);
        dbContext.SaveChanges();
    }

    public bool Delete(User user)
    {
        dbContext.Users.Remove(user);
        dbContext.SaveChanges();
        return true;
    }

    public User GetById(long id)
    {
        return dbContext.Users
                   .Include(e => e.AdditionalInfo)
                   .FirstOrDefault(e => e.Id.Equals(id)) 
               ?? throw new EntityNotFoundException(new User().GetType().Name);
    }

    public User GetByUsername(string username)
    {
        return dbContext.Users
                   .Include(u => u.AdditionalInfo)
                   .FirstOrDefault(u => u.Username.Equals(username)) 
               ?? throw new EntityNotFoundException(new User().GetType().Name);
    }
    
    public User GetFromToken()
    {
        return httpContextAccessor.HttpContext is not null 
            ? GetByUsername(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name)!) 
            : throw new EntityNotFoundException(new User().GetType().Name);
    }

    public void SaveAdditionalInfo(AdditionalUserInfo additionalUserInfo)
    {
        dbContext.AdditionalUserInfos.Add(additionalUserInfo);
        dbContext.SaveChanges();
    }
}