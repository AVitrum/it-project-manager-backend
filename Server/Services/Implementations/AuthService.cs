using System.Security.Authentication;
using EmailService;
using OAuthService;
using Server.Data.Models;
using Server.Exceptions;
using Server.Payload.Requests;
using Server.Payload.Responses;
using Server.Repositories.Interfaces;
using Server.Services.Interfaces;
using UserHelper;
using static UserHelper.UserHelper;
using static UserHelper.PasswordHelper;

namespace Server.Services.Implementations;

public class AuthService(IConfiguration configuration, IEmailSender emailSender, IUserRepository userRepository)
    : IAuthService
{
    public async Task RegisterAsync(RegistrationRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Username))
            throw new ArgumentException("Email and Username are required!");

        if (await userRepository.ExistsByEmailAsync(request.Email))
            throw new ArgumentException("User already exists");

        GeneratePasswordHash(request.Password, out var passwordHash, out var passwordSalt);

        var newUser = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            VerificationToken = CreateRandomToken(),
            RegistrationDate = DateTime.UtcNow,
        };
        await userRepository.CreateAsync(newUser);
    }

    public async Task<LoginResponse> LoginAsync(UserLoginRequest request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email)
            ?? throw new EntityNotFoundException(nameof(User));

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

        GenerateRefreshToken(out var refreshToken);
        await SetRefreshToken(user, refreshToken);

        return new LoginResponse
        {
            AccessToken = GenerateToken(configuration, new UserDto
            {
                Username = user.Username,
                Email = user.Email
            }),
            RefreshToken = refreshToken.Token
        };
    }

    public async Task<bool> GoogleRegisterAsync(GoogleUserInfoResponse googleUserInfoResponse)
    {
        var randomPassword = GeneratePassword(8);

        GeneratePasswordHash(
            randomPassword,
            out var passwordHash,
            out var passwordSalt);

        var newUser = new User
        {
            Email = googleUserInfoResponse.Email!,
            Username = googleUserInfoResponse.Name!,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            RegistrationDate = DateTime.UtcNow,
            VerifiedAt = DateTime.UtcNow,
            PhoneNumber = string.Empty,
        };

        await userRepository.CreateAsync(newUser);

        await emailSender.SendEmailAsync(
            googleUserInfoResponse.Email!,
            "Change your password!",
            $"We have generated a random password for you," +
            " please use it to log in to your profile and change it there: " + $"{randomPassword}");
        return true;
    }

    public async Task<LoginResponse> GoogleLoginAsync(string email)
    {
        var user = await userRepository.GetByEmailAsync(email);

        GenerateRefreshToken(out var refreshToken);

        await SetRefreshToken(user, refreshToken);

        if (!CheckVerificationStatus(new UserDto
        { RegistrationDate = user.RegistrationDate, VerifiedAt = user.VerifiedAt }))
            return new LoginResponse
            {
                AccessToken = GenerateToken(configuration, new UserDto
                {
                    Username = user.Username,
                    Email = user.Email
                }),
                RefreshToken = refreshToken.Token
            };

        user.VerifiedAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);

        return new LoginResponse
        {
            AccessToken = GenerateToken(configuration, new UserDto
            {
                Username = user.Username,
                Email = user.Email
            }),
            RefreshToken = refreshToken.Token
        };
    }

    public async Task SendVerificationToken(string email)
    {
        var user = await userRepository.GetByEmailAsync(email);
        await emailSender.SendEmailAsync(email, "Verification Token", user.VerificationToken!);
    }

    public async Task VerifyAsync(string token)
    {
        var user = await userRepository.GetByTokenAsync(token);
        user.VerifiedAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);
    }

    public async Task<LoginResponse> RefreshAsync(RefreshRequest request)
    {
        var (user, token) = await userRepository.GetByRefreshToken(request.RefreshToken);

        if (token.Expires < DateTime.UtcNow)
        {
            token.Expired = true;
            await userRepository.UpdateRefreshTokenAsync(token);
            throw new AuthenticationException("RefreshToken Expired");
        }

        GenerateRefreshToken(out var refreshToken);
        await SetRefreshToken(user, refreshToken);

        return new LoginResponse
        {
            AccessToken = GenerateToken(configuration, new UserDto
            {
                Username = user.Username,
                Email = user.Email
            }),
            RefreshToken = refreshToken.Token
        };
    }

    public async Task<bool> ExistsByEmail(string email)
    {
        return await userRepository.ExistsByEmailAsync(email);
    }

    private async Task SetRefreshToken(User user, RefreshTokenDto refreshTokenDto)
    {
        var refreshToken = new RefreshToken
        {
            Token = refreshTokenDto.Token,
            Expires = refreshTokenDto.Expires,
            Created = refreshTokenDto.Expires,
            Expired = refreshTokenDto.Expired,
            UserId = user.Id
        };
        await userRepository.UpdateRefreshTokenAsync(refreshToken);
    }
}