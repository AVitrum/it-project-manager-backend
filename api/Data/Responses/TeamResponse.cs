using api.Data.Models;

namespace api.Data.Responses
{
    public class TeamResponse
    {
        public string? Name { get; set; }
        public IEnumerable<UserInfoResponse>? Users { get; set; }

        public static TeamResponse TeamToTeamResponse(Team team)
        {
            var userInfoResponse = team.UserTeams
                .Select(ut => ut.User).ToList()
                .Select(UserInfoResponse.UserToUserInfoResponse).ToList();

            return new TeamResponse()
            {
                Name = team.Name,
                Users = userInfoResponse
            };
        }
    }
}