using api.Data.Requests;
using api.Data.Responses;

namespace api.Services.Interfaces;

public interface IUserService
{
    Task AddInfoAsync(AddInfoRequest request);
    Task SaveImageAsync(IFormFile imageFile);
    Task<bool> DeleteImageAsync();
    Task<UserInfoResponse> ProfileAsync();
}