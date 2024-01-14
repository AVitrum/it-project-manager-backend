using Server.Data.SubModels;
using Server.Data.Models;

namespace Server.Data.Responses;

public class UserInfoResponse
{
    public required long Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required ICollection<AdditionalUserInfoResponse> InfoResponses { get; set; }

    public static UserInfoResponse UserToUserInfoResponse(User user)
    {
        var infoResponses =
            user.AdditionalInfo.Select(AdditionalUserInfoResponse.ToResponse).ToList();
        
        return new UserInfoResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            InfoResponses = infoResponses
        };
    }
}