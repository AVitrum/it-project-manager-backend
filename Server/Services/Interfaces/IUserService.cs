using Server.Payload.Requests;
using Server.Payload.Responses;

namespace Server.Services.Interfaces;

public interface IUserService
{
    Task<UserInfoResponse> UserProfileAsync();
    Task ChangeProfileImage(IFormFile imageUrl);
    Task CreateResetPasswordTokenAsync(string email);
    Task<string> ChangePasswordAsync(ChangePasswordRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
}