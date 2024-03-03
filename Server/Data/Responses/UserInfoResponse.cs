using Server.Data.Models;

namespace Server.Data.Responses;

public class UserInfoResponse
{
    public required long Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required DateTime CreationDate { get; set; }
    public required bool IsBlocked { get; set; }

    public static UserInfoResponse UserToUserInfoResponse(User user)
    {
        return new UserInfoResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreationDate = user.CreationDate,
            IsBlocked = user.IsBlocked
        };
    }
}