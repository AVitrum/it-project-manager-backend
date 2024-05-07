using DatabaseService.Data.Models;
using DatabaseService.Repositories.Interfaces;
using EmailService;
using FileService;
using Server.Payload.Requests;
using Server.Payload.Responses;
using Server.Services.Interfaces;
using UserHelper;
using static UserHelper.UserHelper;
using static UserHelper.PasswordHelper;

namespace Server.Services.Implementations;

public class UserService(
    IEmailSender emailSender,
    IFileService fileService,
    IUserRepository userRepository)
    : IUserService
{
    public async Task UpdateUser(UserDto userDto)
    {
        var currentUser = await userRepository.GetByJwtAsync();

        if (userDto.Phone != null)
        {
            currentUser.PhoneNumber = userDto.Phone;
        }

        if (!string.IsNullOrEmpty(userDto.Username))
        {
            currentUser.Username = userDto.Username;
        }

        await userRepository.UpdateAsync(currentUser);
    }

    public async Task<UserInfoResponse> UserProfileAsync()
    {
        var (user, picture) = await userRepository.GetByJwtWithPhotoAsync();

        var profile = new UserInfoResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreationDate = user.RegistrationDate,
            PhoneNumber = user.PhoneNumber,
        };

        if (picture != null)
            profile.ImageUrl = picture.PictureLink;

        return profile;
    }

    public async Task ChangeProfileImage(IFormFile file)
    {
        var (user, picture) = await userRepository.GetByJwtWithPhotoAsync();
        
        fileService.CheckImage(file);

        if (picture != null)
            await fileService.DeleteAsync(picture.PictureName);

        var imageUrl = await fileService.UploadAsync(user.Username, file);

        var newPicture = new ProfilePhoto
        {
            PictureLink = imageUrl,
            PictureName = $"{user.Username}_{file.FileName}",
            UserId = user.Id
        };

        await userRepository.AddProfilePhoto(newPicture);
    }

    public async Task CreateResetPasswordTokenAsync(string email)
    {
        var user = await userRepository.GetByEmailAsync(email);

        user.PasswordResetToken = CreateRandomToken();
        user.ResetTokenExpires = DateTime.UtcNow.AddDays(1);
        await userRepository.UpdateAsync(user);

        await emailSender.SendEmailAsync(email, "Password reset token", user.PasswordResetToken);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
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

    public async Task<string> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var user = await userRepository.GetByJwtAsync();

        GeneratePasswordHash(request.NewPassword, out var passwordHash, out var passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        await userRepository.UpdateAsync(user);

        await emailSender.SendEmailAsync(
            user.Email,
            "Password",
            "Your password has been changed");
        
        return "Updated";
    }
}