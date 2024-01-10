using api.Data.SubModels;

namespace api.Data.Responses;

public class UserTeamResponse
{
    public required long Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }

    public static UserTeamResponse UserTeamToUserTeamResponse(UserTeam userTeam)
    {
        return new UserTeamResponse
        {
            Id = userTeam.User.Id,
            Username = userTeam.User.Username,
            Email = userTeam.User.Email,
            Role = userTeam.Role.ToString()
        };
    }
}