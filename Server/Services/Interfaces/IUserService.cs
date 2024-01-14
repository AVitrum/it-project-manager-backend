using Server.Data.Requests;
using Server.Data.Responses;

namespace Server.Services.Interfaces;

public interface IUserService
{
    Task AddInfoAsync(AddInfoRequest request);
    Task SaveImageAsync(IFormFile imageFile);
    Task<bool> DeleteImageAsync();
    Task<UserInfoResponse> ProfileAsync();
}