using System.Security.Claims;
using api.Context;
using api.Data.Models;
using api.Data.SubModels;
using api.Exceptions;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Services.Implementations;

public class UserService(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext) : IUserService
{
    public void CreateUser(User user)
    {
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
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

    public void AddInfo(AdditionalUserInfo additionalUserInfo)
    {
        dbContext.AdditionalUserInfos.Add(additionalUserInfo);
        dbContext.SaveChanges();
    }
}