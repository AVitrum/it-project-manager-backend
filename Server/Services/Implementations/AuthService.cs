using Server.Data.Requests;
using Server.Exceptions;
using Server.Repositories.Interfaces;
using Server.Services.Interfaces;
using UserHelper;
using static UserHelper.UserHelper;

namespace Server.Services.Implementations;

public class AuthService(IConfiguration configuration, IUserRepository userRepository) : IAuthService
{
    public async Task RegisterAsync(UserCreationRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ArgumentException("Email and Username are required!");
        }

        var user = UserCreationRequest.UserCreationRequestToUser(request);
        await userRepository.CreateAsync(user);
    }

    public async Task<string> LoginAsync(UserLoginRequest request)
    {
        var user = await userRepository.GetAsync(request.Email);

        CheckHashedPassword(request.Password, user.PasswordHash);
        BanCheck(user.IsBlocked);
        if (!CheckDateAfterRegistration(user.CreationDate))
            return CreateToken(configuration, new UserDto
            {
                Username = user.Username,
                Email = user.Email
            });
        await userRepository.DeleteAsync(user);
        throw new EntityNotFoundException("Your account has been deleted!");

    }
}