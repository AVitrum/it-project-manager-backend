using System.Security.Claims;
using api.Context;
using api.Data.Models;
using api.Data.SubModels;
using api.Services.Interfaces;

namespace api.Services.Implementations;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _dbContext;

    public UserService(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }


    public void CreateUser(User user)
    {
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
    }

    public User GetById(long id)
    {
        return _dbContext.Users.FirstOrDefault(x => x.Id.Equals(id)) 
               ?? throw new ArgumentException("User not found");
    }
    
    public User GetByUsername(string username)
    {
        return _dbContext.Users.FirstOrDefault(x => x.Username.Equals(username)) 
               ?? throw new ArgumentException("User not found");
    }

    public string GetName()
    {
        var result = string.Empty;
        if(_httpContextAccessor.HttpContext is not null)
        {
            result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }
        return result;
    }
}