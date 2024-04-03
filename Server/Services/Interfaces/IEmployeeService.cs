using Server.Data.Requests;
using Server.Data.Responses;
using UserService;
using UserService.Payload.Requests;
using UserService.Payload.Responses;

namespace Server.Services.Interfaces;

public interface IEmployeeService
{
    Task<UserInfoResponse> ProfileAsync();
    Task CreateResetPasswordTokenAsync(string email);
    Task<string> ChangePasswordAsync(ChangePasswordRequest request);
    Task ResetPassword(ResetPasswordRequest request);
}