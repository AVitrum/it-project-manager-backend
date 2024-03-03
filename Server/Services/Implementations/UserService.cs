using Server.Data.Responses;
using Server.Repositories.Interfaces;
using Server.Services.Interfaces;
using UserHelper;
using static UserHelper.UserHelper;

namespace Server.Services.Implementations;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<UserInfoResponse> ProfileAsync()
    {
        var user = await userRepository.GetAsync();
        return UserInfoResponse.UserToUserInfoResponse(user);
    }

    public async Task<string> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var user = await userRepository.GetAsync();

        CheckPassword(request);

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await userRepository.UpdateAsync(user);
        return "Updated";
    }

    public string GenerateCodeAsync(string email)
    {
        var code = new Random().Next(100000, 1000000);
        
        // Add implementation of code saving
        
        return code.ToString();
    }
}