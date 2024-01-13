using api.Data.Requests;
using api.Data.Responses;
using api.Data.SubModels;
using api.Repositories.Interfaces;
using api.Services.Interfaces;
using FileStream = System.IO.FileStream;

namespace api.Services.Implementations;

public class UserService(IHostEnvironment env, IUserRepository userRepository) : IUserService
{
    public void AddInfo(AddInfoRequest request)
    {
        var user = userRepository.GetFromToken();
        
        var additionalUserInfo = new AdditionalUserInfo
        {
            UserId = user.Id,
            Type = request.Type,
            Info = request.Info,
        };
        
        userRepository.SaveAdditionalInfo(additionalUserInfo);
    }

    public void SaveImage(IFormFile imageFile)
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
        imageFile.CopyTo(stream);
        stream.Close();

        var user = userRepository.GetFromToken();
        user.Image = newFileName;
        user.ImageFile = imageFile;
        userRepository.Update(user);
    }

    public bool DeleteImage()
    {
        var user = userRepository.GetFromToken();
        
        var path = Path.Combine(env.ContentRootPath, "Uploads/", user.Image!);
        Console.WriteLine(path);

        if (!File.Exists(path))
        {
            return false;
        }
        
        user.Image = null;
        user.ImageFile = null;
        userRepository.Update(user);
       
        File.Delete(path);
        return true;
    }

    public UserInfoResponse Profile()
    {
        return UserInfoResponse.UserToUserInfoResponse(userRepository.GetFromToken());
    }
}