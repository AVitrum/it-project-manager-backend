using EmailSystem;
using Server.Data.Models;
using Server.Data.Requests;
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

        if (await userRepository.ExistByEmailAsync(request.Email))
        {
            throw new ArgumentException("User already exists");
        }
        
        CreatePasswordHash(request.Password, out var passwordHash, out var passwordSalt);

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            VerificationToken = CreateRandomToken(),
            CreationDate = DateTime.UtcNow,
        };
        await userRepository.CreateAsync(user);
    }


    public async Task<string> LoginAsync(UserLoginRequest request)
    {
        var user = await userRepository.GetAsync(request.Email);

        if (user == null)
        {
            throw new EntityNotFoundException(nameof(User));
        }

        if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            throw new ArgumentException("Password is incorrect");
        }

        if (user.VerifiedAt == null)
        {
            throw new ArgumentException("Not verified!");
        }
        
        return CreateToken(configuration, new UserDto
        {
            Username = user.Username,
            Email = user.Email
        });
    }

    public async Task SendVerificationToken(string email)
    {
        var user = await userRepository.GetAsync(email);
        await emailSender.SendEmailAsync(email, "Verification Token", user.VerificationToken!);
    }
    
    public async Task Verify(string token)
    {
        var user = await userRepository.GetAsyncByToken(token);
        user.VerifiedAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);
    }
}