using api.Data.Enums;
using api.Data.Models;
using api.Data.Requests;
using api.Data.Responses;
using api.Data.SubModels;
using api.Repositories.Interfaces;
using api.Services.Interfaces;

namespace api.Services.Implementations;

public class TeamService(ITeamRepository teamRepository, IUserRepository userRepository) : ITeamService
{
    public void Create(TeamCreationRequest request)
    {
        var team = new Team
        {
            Name = request.Name
        };

        teamRepository.Create(team);

        var user = userRepository.GetFromToken();
        
        var userTeam = new UserTeam 
        {
            UserId = user.Id,
            TeamId = team.Id,
            Role = UserRole.Manager
        };
        
        teamRepository.SaveUserInTeam(userTeam);
    }

    public void AddUser(long teamId, long userId)
    {
        var team = teamRepository.GetById(teamId);
        var user = userRepository.GetById(userId);
        
        if (HasPermission(userRepository.GetFromToken(), team))
        {
            throw new Exception("Server error.");
        }
        
        if (InTeam(user, team))
        {
            throw new ArgumentException("User already in this team");
        }
        
        var userTeam = new UserTeam
        {
            UserId = user.Id,
            TeamId = team.Id,
            Role = UserRole.Regular
        };

        teamRepository.SaveUserInTeam(userTeam);
    }
    
    public TeamResponse Get(long id)
    {
        return TeamResponse.TeamToTeamResponse(teamRepository.GetById(id));
    }  

    public bool HasPermission(User user, Team team)
    {
        var userTeam = teamRepository.FindByUserAndTeam(user, team);
        return userTeam is { Role: UserRole.Manager };
    }
    
    private bool InTeam(User user, Team team)
    {
        var userTeam = teamRepository.FindByUserAndTeam(user, team);
        return userTeam != null;
    }
}