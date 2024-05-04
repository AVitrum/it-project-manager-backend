using Server.Payload.Requests;
using Server.Payload.Responses;
using UserHelper;

namespace Server.Services.Interfaces;

public interface IUserService
{
    Task UpdateUser(UserDto userDto);
    Task<UserInfoResponse> UserProfileAsync();
    Task ChangeProfileImage(IFormFile imageUrl);
    Task CreateResetPasswordTokenAsync(string email);
    Task<string> ChangePasswordAsync(ChangePasswordRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
}