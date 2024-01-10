using System.Security.Authentication;
using System.Security.Claims;
using api.Context;
using api.Data.Models;
using api.Data.SubModels;
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
                   .Include(u => u.AdditionalInfo)
                   .FirstOrDefault(u => u.Id.Equals(id)) 
               ?? throw new ArgumentException("User not found");
    }
    
    public User GetByUsername(string username)
    {
        return dbContext.Users
                   .Include(u => u.AdditionalInfo)
                   .FirstOrDefault(u => u.Username.Equals(username)) 
               ?? throw new ArgumentException("User not found");
    }

    public User GetFromToken()
    {
        if(httpContextAccessor.HttpContext is not null)
        {
            return GetByUsername(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name) 
                                 ?? throw new AuthenticationException("Wrong token"));
        }
        return null!;
    }

    public void AddInfo(AdditionalUserInfo additionalUserInfo)
    {
        dbContext.AdditionalUserInfos.Add(additionalUserInfo);
        dbContext.SaveChanges();
    }
}