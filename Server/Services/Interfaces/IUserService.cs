using Server.Data.Requests;
using Server.Data.Responses;
using UserHelper;
using UserHelper.Payload.Requests;
using UserHelper.Responses;

namespace Server.Services.Interfaces;

public interface IUserService
{
    Task<UserInfoResponse> ProfileAsync();
    Task CreateResetPasswordTokenAsync(string email);
    Task<string> ChangePasswordAsync(ChangePasswordRequest request);
    Task ResetPassword(ResetPasswordRequest request);
}