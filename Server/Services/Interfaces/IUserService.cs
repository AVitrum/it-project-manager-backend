using Server.Payload.Requests;
using Server.Payload.Responses;
using UserHelper;

namespace Server.Services.Interfaces;

public interface IUserService
{
    Task<UserInfoResponse> ProfileAsync();
    Task CreateResetPasswordTokenAsync(string email);
    Task<string> ChangePasswordAsync(ChangePasswordRequest request);
    Task ResetPassword(ResetPasswordRequest request);
}