using System.Text.RegularExpressions;

namespace UserHelper;

public partial class PasswordGenerator
{
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