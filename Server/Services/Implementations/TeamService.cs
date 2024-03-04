using Server.Data.Enums;
using Server.Data.Models;
using Server.Data.Requests;
using Server.Data.Responses;
using Server.Repositories.Interfaces;
using Server.Services.Interfaces;

namespace Server.Services.Implementations;

public class TeamService(ITeamRepository teamRepository, IUserRepository userRepository) : ITeamService
{
    public async Task CreateAsync(TeamCreationRequest request)
    {
        var team = new Team
        {
            Name = request.Name
        };

        await teamRepository.CreateAsync(team);

        var user = await userRepository.GetAsync();

        var userTeam = new UserTeam
        {
            UserId = user.Id,
            TeamId = team.Id,
            Role = UserRole.Manager
        };

        await teamRepository.SaveUserInTeamAsync(userTeam);
    }

    public async Task AddUserAsync(long teamId, long userId)
    {
        var team = await teamRepository.GetAsync(teamId);
        var user = await userRepository.GetAsync(userId);

        if (await HasPermissionAsync(await userRepository.GetAsync(), team))
        {
            throw new Exception("Server error.");
        }

        if (await InTeamAsync(user, team))
        {
            throw new ArgumentException("User already in this team");
        }

        var userTeam = new UserTeam
        {
            UserId = user.Id,
            TeamId = team.Id,
            Role = UserRole.Regular
        };

        await teamRepository.SaveUserInTeamAsync(userTeam);
    }

    public async Task<TeamResponse> GetAsync(long id)
    {
        return TeamResponse.TeamToTeamResponse(await teamRepository.GetAsync(id));
    }

    private async Task<bool> HasPermissionAsync(User user, Team team)
    {
        var userTeam = await teamRepository.FindByUserAndTeamAsync(user, team);
        return userTeam is { Role: UserRole.Manager };
    }

    private async Task<bool> InTeamAsync(User user, Team team)
    {
        var userTeam = await teamRepository.FindByUserAndTeamAsync(user, team);
        return userTeam != null;
    }
}