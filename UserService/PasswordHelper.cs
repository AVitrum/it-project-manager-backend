using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace UserService;

public static partial class PasswordHelper
{
    public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }
    
    public static void GeneratePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
    
    [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")]
    private static partial Regex MyRegex();
    private static readonly Random Random = new Random();
    private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+[]{}|;:',.<>?";

    public static string GeneratePassword(int length)
    {
        var password = new char[length];
        for (var i = 0; i < length; i++)
        {
            password[i] = Chars[Random.Next(Chars.Length)];
        }
        return new string(password);
    }

    public static bool IsStrongPassword(string password)
    {
        return MyRegex().IsMatch(password);
    }
}