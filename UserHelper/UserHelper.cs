using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace UserHelper;

public static class UserHelper
{
    public static string CreateRandomToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }
    
    public static string GenerateBearerToken(IConfiguration configuration, UserDto user)
    {
        var claims = new List<Claim> {
            new(ClaimTypes.Name, user.Username!),
            new(ClaimTypes.Email, user.Email!),
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

    public static void GenerateRefreshToken(out RefreshTokenDto refreshTokenDto)
    {
        refreshTokenDto = new RefreshTokenDto
        {
            Token = CreateRandomToken(),
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(7),
            Expired = false
        };
    }

    public static bool IsAccountVerified(UserDto user)
    {
        return user.VerifiedAt == null && user.RegistrationDate!.Value.Date.AddDays(3) < DateTime.UtcNow;
    }
}