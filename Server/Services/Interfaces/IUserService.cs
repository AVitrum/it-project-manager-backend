using Server.Data.Responses;
using UserHelper;

namespace Server.Services.Interfaces;

public interface IUserService
{
    Task<UserInfoResponse> ProfileAsync();
    Task<string> ChangePasswordAsync(ChangePasswordRequest request);
    string GenerateCodeAsync(string email);
}