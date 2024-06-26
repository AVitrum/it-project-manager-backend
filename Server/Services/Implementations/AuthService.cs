using System.Security.Authentication;
using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using EmailService;
using OAuthService;
using Server.Payload.Requests;
using Server.Payload.Responses;
using Server.Services.Interfaces;
using UserHelper;
using static UserHelper.UserHelper;
using static UserHelper.PasswordHelper;

namespace Server.Services.Implementations;

public class AuthService(
    IConfiguration configuration,
    IEmailSender emailSender,
    IUserRepository userRepository)
    : IAuthService
{
    public async Task RegisterAsync(RegistrationRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Username))
            throw new UserException("Email and Username are required!");

        if (await userRepository.ExistsByEmailAsync(request.Email)
            || await userRepository.ExistsByUsernameAsync(request.Username))
            throw new NotUniqueEmailException("User already exists");

        GeneratePasswordHash(request.Password, out var passwordHash, out var passwordSalt);

        var newUser = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            VerificationToken = CreateRandomToken(),
            RegistrationDate = DateTime.UtcNow.AddHours(3)
        };
        await userRepository.CreateAsync(newUser);
    }

    public async Task<LoginResponse> LoginAsync(UserLoginRequest request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);

        if (!IsPasswordHashEqual(request.Password, user.PasswordHash, user.PasswordSalt))
            throw new UserException("Password is incorrect");

        if (IsAccountVerified(new UserDto
        { RegistrationDate = user.RegistrationDate, VerifiedAt = user.VerifiedAt }))
        {
            await userRepository.DeleteAsync(user);
            throw new UserException("You have not verified your account." +
                                    " Your account has been deleted");
        }

        if (user.VerifiedAt == null) throw new UserException("Not verified! Please verify your account");

        GenerateRefreshToken(out var refreshToken);
        await SetRefreshToken(user, refreshToken);

        user.GoogleAccessToken = string.Empty;
        await userRepository.UpdateAsync(user);

        return new LoginResponse
        {
            AccessToken = GenerateBearerToken(configuration,
                new UserDto
                {
                    Username = user.Username,
                    Email = user.Email
                }),
            RefreshToken = refreshToken.Token,
        };
    }

    public async Task<bool> GoogleOAuthRegistrationAsync(GoogleUserInfoResponse googleUserInfoResponse)
    {
        var randomPassword = GeneratePassword(8);

        GeneratePasswordHash(
            randomPassword,
            out var passwordHash,
            out var passwordSalt);

        var newUser = new User
        {
            Email = googleUserInfoResponse.Email,
            Username = googleUserInfoResponse.Name + "#" + new Random().Next(9999, 999999),
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            RegistrationDate = DateTime.UtcNow.AddHours(3),
            VerifiedAt = DateTime.UtcNow,
            PhoneNumber = string.Empty,
        };

        await userRepository.CreateAsync(newUser);

        await emailSender.SendEmailAsync(
            googleUserInfoResponse.Email,
            "Change your password!",
            $"We have generated a random password for you," +
            " please use it to log in to your profile and change it there: " + $"{randomPassword}");
        return true;
    }

    public async Task<LoginResponse> GoogleOAuthLoginAsync(string email, string token)
    {
        var user = await userRepository.GetByEmailAsync(email);

        user.GoogleAccessToken = token;
        await userRepository.UpdateAsync(user);

        GenerateRefreshToken(out var refreshToken);

        await SetRefreshToken(user, refreshToken);

        if (!IsAccountVerified(new UserDto
        { RegistrationDate = user.RegistrationDate, VerifiedAt = user.VerifiedAt }))
            return new LoginResponse
            {
                AccessToken = GenerateBearerToken(configuration,
                    new UserDto
                    {
                        Username = user.Username,
                        Email = user.Email
                    }),
                RefreshToken = refreshToken.Token,
            };

        user.VerifiedAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);

        return new LoginResponse
        {
            AccessToken = GenerateBearerToken(configuration,
                new UserDto
                {
                    Username = user.Username,
                    Email = user.Email
                }),
            RefreshToken = refreshToken.Token,
        };
    }

    public async Task SendVerificationToken(string email)
    {
        var user = await userRepository.GetByEmailAsync(email);
        await emailSender.SendEmailAsync(email, "Verification Token", user.VerificationToken!);
    }

    public async Task VerifyAccountAsync(string token)
    {
        var user = await userRepository.GetByTokenAsync(token);
        user.VerifiedAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);
    }

    public async Task<LoginResponse> RefreshJwtAsync(string refreshToken)
    {
        var (user, token) = await userRepository.GetByRefreshToken(refreshToken);

        if (token.Expires < DateTime.UtcNow)
        {
            token.Expired = true;
            await userRepository.UpdateRefreshTokenAsync(token);
            throw new AuthenticationException("RefreshToken Expired");
        }

        GenerateRefreshToken(out var newRefreshToken);
        await SetRefreshToken(user, newRefreshToken);

        return new LoginResponse
        {
            AccessToken = GenerateBearerToken(configuration,
                new UserDto
                {
                    Username = user.Username,
                    Email = user.Email
                }),
            RefreshToken = newRefreshToken.Token,
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