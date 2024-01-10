using api.Context;
using api.Data.Enums;
using api.Data.Models;
using api.Data.Requests;
using api.Data.SubModels;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Services.Implementations;

public class TeamService(AppDbContext dbContext) : ITeamService
{
    public void Create(TeamCreationRequest request, User user)
    {
        var team = new Team
        {
            Name = request.Name
        };

        dbContext.Teams.Add(team);
        dbContext.SaveChanges();

        var userTeam = new UserTeam 
        {
            UserId = user.Id,
            TeamId = team.Id,
            Role = UserRole.Manager
        };

        dbContext.UserTeams.Add(userTeam);
        dbContext.SaveChanges();
    }

    public bool AddUser(User user, Team team, UserRole role)
    {
        if (user.Equals(null) || team.Equals(null)) return false;
        if (InTeam(user, team)) throw new ArgumentException("User already in this team");
        
        var userTeam = new UserTeam
        {
            UserId = user.Id,
            TeamId = team.Id,
            Role = role
        };

        dbContext.UserTeams.Add(userTeam);
        dbContext.SaveChanges();
        return true;
    }
    
    public Team Get(long id)
    {
        return dbContext.Teams
                   .Include(t => t.UserTeams) 
                   .ThenInclude(ut => ut.User)
                   .ThenInclude(u => u.AdditionalInfo)
                   .FirstOrDefault(t => t.Id == id)
               ?? throw new ArgumentException("Team not found");
    }  

    public bool HasPermission(User user, Team team)
    {
        var userTeam = FindByUserAndTeam(user, team);
        return userTeam is { Role: UserRole.Manager };
    }

    private UserTeam? FindByUserAndTeam(User user, Team team)
    {
        var userTeam = dbContext.UserTeams
            .FirstOrDefault(ut => ut.UserId == user.Id && ut.TeamId == team.Id);
        return userTeam;
    }
    
    private bool InTeam(User user, Team team)
    {
        var userTeam = FindByUserAndTeam(user, team);
        return userTeam != null;
    }
}