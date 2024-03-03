using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace UserHelper;

public static class UserHelper
{
    public static void CheckHashedPassword(string passwordToCheck, string userPasswordHash)
    {
        if(!BCrypt.Net.BCrypt.Verify(passwordToCheck, userPasswordHash)) 
            throw new AuthenticationException("Wrong password!");
    }

    public static void CheckPassword(ChangePasswordRequest request)
    { 
        if (!request.NewPassword.Equals(request.SecondNewPassword)) 
            throw new ArgumentException("Passwords are not the same!");
    }

    public static bool CheckDateAfterRegistration(DateTime registrationDate)
    {
        return registrationDate.AddDays(3) >= DateTime.Now;
    }
    
    public static void BanCheck(bool isBanned)
    {
        if (isBanned) throw new ArgumentException("You need to verify your account!");
    }
    
    public static string CreateToken(IConfiguration configuration, UserDto user)
    {
        var claims = new List<Claim> {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, "Admin"),
            new(ClaimTypes.Role, "User")
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