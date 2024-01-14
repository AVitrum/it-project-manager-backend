using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Server.Data.Models;
using Server.Data.Requests;
using Server.Repositories.Interfaces;
using Server.Services.Interfaces;

namespace Server.Services.Implementations;

public class AuthService(IConfiguration configuration, IUserRepository userRepository) : IAuthService
{
    public async Task RegisterAsync(UserCreationRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ArgumentException("Email and Username are required!");
        }

        var user = UserCreationRequest.UserCreationRequestToUser(request);
        await userRepository.CreateAsync(user);
    }

    public async Task<string> LoginAsync(UserLoginRequest request)
    {
        var user = await userRepository.GetAsync(request.Username);

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new AuthenticationException("Wrong password");
        }

        return CreateToken(user);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim> {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, "Admin"),
            new(ClaimTypes.Role, "User"),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            configuration.GetSection("AppSettings:Token").Value!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}