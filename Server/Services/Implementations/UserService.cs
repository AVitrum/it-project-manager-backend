using EmailSystem;
using Server.Repositories.Interfaces;
using Server.Services.Interfaces;
using UserHelper.Payload.Requests;
using UserHelper.Responses;
using static UserHelper.UserHelper;

namespace Server.Services.Implementations;

public class UserService(IEmailSender emailSender, IUserRepository userRepository) : IUserService
{
    public async Task<UserInfoResponse> ProfileAsync()
    {
        var user = await userRepository.GetByCurrentTokenAsync();
        return new UserInfoResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreationDate = user.RegistrationDate,
        };
    }

    public async Task CreateResetPasswordTokenAsync(string email)
    {
        var user = await userRepository.GetByEmailAsync(email);

        user.PasswordResetToken = CreateRandomToken();
        user.ResetTokenExpires = DateTime.UtcNow.AddDays(1);
        await userRepository.UpdateAsync(user);

        await emailSender.SendEmailAsync(email, "Password reset token", user.PasswordResetToken);
    }

    public async Task<string> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var user = await userRepository.GetByCurrentTokenAsync();

        GeneratePasswordHash(request.NewPassword, out var passwordHash, out var passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        await userRepository.UpdateAsync(user);
        return "Updated";
    }

    public async Task ResetPassword(ResetPasswordRequest request)
    {
        var user = await userRepository.GetByPasswordResetTokenAsync(request.Token);

        if (user.ResetTokenExpires < DateTime.UtcNow) throw new ArgumentException("The token is not valid!");

        GeneratePasswordHash(request.Password, out var passwordHash, out var passwordSalt);
        
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        user.PasswordResetToken = null;
        user.ResetTokenExpires = null;
        
        await userRepository.UpdateAsync(user);
    }
}