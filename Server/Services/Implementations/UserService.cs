using DatabaseService.Data.Models;
using EmailService;
using FileService;
using Server.Payload.Requests;
using Server.Payload.Responses;
using Server.Repositories.Interfaces;
using Server.Services.Interfaces;
using static UserHelper.UserHelper;
using static UserHelper.PasswordHelper;

namespace Server.Services.Implementations;

public class UserService(
    IEmailSender emailSender,
    IFileService fileService,
    IUserRepository userRepository) 
    : IUserService
{
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
        {
            profile.ImageUrl = picture.PictureLink;
        }

        return profile;
    }

    public async Task ChangeProfileImage(IFormFile file)
    {
        var (user, picture) = await userRepository.GetByJwtWithPhotoAsync();

        if (file.Length > 5e+6)
        {
            throw new FileToLargeException("The file size must be less than 5 MB!");
        }
        
        var imageFormats = new HashSet<string>
        {
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/bmp",
        };

        if (!imageFormats.Contains(file.ContentType))
        {
            throw new FileInvalidFormatException("Unsupported picture format!");
        }
        
        if (picture != null)
        {
            await fileService.DeleteAsync(picture.PictureName);
        }
        
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
        return "Updated";
    }
    
}