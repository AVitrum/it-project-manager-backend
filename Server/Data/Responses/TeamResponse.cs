using Server.Data.Models;

namespace Server.Data.Responses;

public class TeamResponse
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public required List<UserTeamResponse?> Users { get; set; }

    public static TeamResponse TeamToTeamResponse(Team team)
    {
        var userInfoResponse = team.UserTeams
            .Select(UserTeamResponse.UserTeamToUserTeamResponse).ToList();

        return new TeamResponse()
        {
            Id = team.Id,
            Name = team.Name,
            Users = userInfoResponse!
        };
    }
}