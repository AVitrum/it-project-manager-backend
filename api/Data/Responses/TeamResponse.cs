using api.Data.Models;

namespace api.Data.Responses;

public class TeamResponse
{
    public required string Name { get; set; }
    public required List<UserTeamResponse?> Users { get; set; }

    public static TeamResponse TeamToTeamResponse(Team team)
    {
        var userInfoResponse = team.UserTeams
            .Select(UserTeamResponse.UserTeamToUserTeamResponse).ToList();

        return new TeamResponse()
        {
            Name = team.Name,
            Users = userInfoResponse!
        };
    }
}