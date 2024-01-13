using api.Data.Requests;
using api.Data.Responses;

namespace api.Services.Interfaces;

public interface IUserService
{
    void AddInfo(AddInfoRequest request);
    void SaveImage(IFormFile imageFile);
    bool DeleteImage();
    UserInfoResponse Profile();
}