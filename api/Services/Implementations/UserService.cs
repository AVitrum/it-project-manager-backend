using api.Data.Requests;
using api.Data.Responses;
using api.Data.SubModels;
using api.Repositories.Interfaces;
using api.Services.Interfaces;

namespace api.Services.Implementations;

public class UserService(IHostEnvironment env, IUserRepository userRepository) : IUserService
{
    public async Task AddInfoAsync(AddInfoRequest request)
    {
        var user = await userRepository.GetAsync();

        var additionalUserInfo = new AdditionalUserInfo
        {
            UserId = user.Id,
            Type = request.Type,
            Info = request.Info,
        };

        await userRepository.SaveAdditionalInfoAsync(additionalUserInfo);
    }

    public async Task SaveImageAsync(IFormFile imageFile)
    {
        var contentPath = env.ContentRootPath;
        var path = Path.Combine(contentPath, "Uploads");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var ext = Path.GetExtension(imageFile.FileName);
        var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };

        if (!allowedExtensions.Contains(ext))
        {
            throw new ArgumentException($"The file is in an incorrect format, acceptable formats:{string.Join(",", allowedExtensions)}");
        }

        var newFileName = Guid.NewGuid() + ext;
        var stream = new FileStream(Path.Combine(path, newFileName), FileMode.Create);
        await imageFile.CopyToAsync(stream);
        stream.Close();

        var user = await userRepository.GetAsync();
        user.Image = newFileName;
        user.ImageFile = imageFile;
        await userRepository.UpdateAsync(user);
    }

    public async Task<bool> DeleteImageAsync()
    {
        var user = await userRepository.GetAsync();

        var path = Path.Combine(env.ContentRootPath, "Uploads/", user.Image!);
        Console.WriteLine(path);

        if (!File.Exists(path))
        {
            return false;
        }

        user.Image = null;
        user.ImageFile = null;
        await userRepository.UpdateAsync(user);

        File.Delete(path);
        return true;
    }

    public async Task<UserInfoResponse> ProfileAsync()
    {
        var user = await userRepository.GetAsync();
        return UserInfoResponse.UserToUserInfoResponse(user);
    }
}