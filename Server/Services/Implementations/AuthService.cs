using EmailSystem;
using OAuth;
using Server.Data.Models;
using Server.Exceptions;
using Server.Repositories.Interfaces;
using Server.Services.Interfaces;
using UserHelper;
using UserHelper.Payload.Requests;
using static UserHelper.UserHelper;

namespace Server.Services.Implementations;

public class AuthService(IConfiguration configuration, IEmailSender emailSender, IUserRepository userRepository) 
    : IAuthService 
{
    public async Task RegisterAsync(UserCreationRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ArgumentException("Email and Username are required!");
        }

        if (await userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new ArgumentException("User already exists");
        }
        
        GeneratePasswordHash(request.Password, out var passwordHash, out var passwordSalt);

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            VerificationToken = CreateRandomToken(),
            RegistrationDate = DateTime.UtcNow,
            PhoneNumber = request.PhoneNumber,
        };
        await userRepository.CreateAsync(user);
    }


    public async Task<string> LoginAsync(UserLoginRequest request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);

        if (user == null) throw new EntityNotFoundException(nameof(User));

        if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt)) 
            throw new ArgumentException("Password is incorrect");

        if (CheckVerificationStatus(new UserDto
                { RegistrationDate = user.RegistrationDate, VerifiedAt = user.VerifiedAt }))
        {
            await userRepository.DeleteAsync(user);
            throw new EntityNotFoundException("You have not verified your account." +
                                              " Your account has been deleted");
        }

        if (user.VerifiedAt == null) throw new ArgumentException("Not verified! Please verify your account");
        
        return GenerateToken(configuration, new UserDto
        {
            Username = user.Username,
            Email = user.Email
        });
    }

    public async Task GoogleRegisterAsync(GoogleUserInfoResponse googleUserInfoResponse)
    {
        var randomPassword = PasswordGenerator.GeneratePassword(8);
        
        GeneratePasswordHash(
            randomPassword,
            out var passwordHash,
            out var passwordSalt);

        var user = new User
        {
            Email = googleUserInfoResponse.Email!,
            Username = googleUserInfoResponse.Name!,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            RegistrationDate = DateTime.UtcNow,
            VerifiedAt = DateTime.UtcNow,
            PhoneNumber = string.Empty,
        };

        await userRepository.CreateAsync(user);

        await emailSender.SendEmailAsync(
            googleUserInfoResponse.Email!,
            "Change your password!",
            $"We have generated a random password for you," +
            " please use it to log in to your profile and change it there: " + $"{randomPassword}");
    }

    public async Task<string> GoogleLoginAsync(string email)
    {
        var user = await userRepository.GetByEmailAsync(email);

        if (!CheckVerificationStatus(new UserDto
                { RegistrationDate = user.RegistrationDate, VerifiedAt = user.VerifiedAt }))
            return GenerateToken(configuration, new UserDto
            {
                Username = user.Username,
                Email = user.Email
            });
        user.VerifiedAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);

        return GenerateToken(configuration, new UserDto
        {
            Username = user.Username,
            Email = user.Email
        });
    }

    public async Task<bool> ExistsByEmail(string email)
    {
        return await userRepository.ExistsByEmailAsync(email);
    }

    public async Task SendVerificationToken(string email)
    {
        var user = await userRepository.GetByEmailAsync(email);
        await emailSender.SendEmailAsync(email, "Verification Token", user.VerificationToken!);
    }
    
    public async Task Verify(string token)
    {
        var user = await userRepository.GetByTokenAsync(token);
        user.VerifiedAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);
    }
}