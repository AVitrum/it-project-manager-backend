using api.Context;
using api.Data.Models;
using api.Data.Requests;
using api.Data.SubModels;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Services.Implementations;

public class TeamService : ITeamService
{
    private readonly AppDbContext _dbContext;

    public TeamService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Create(TeamCreationRequest request)
    {
        var team = new Team
        {
            Name = request.Name
        };
        _dbContext.Teams.Add(team);
        _dbContext.SaveChanges();
    }

    public Team Get(long id)
    {
        return _dbContext.Teams
                   .Include(t => t.UserTeams) 
                   .ThenInclude(ut => ut.User) 
                   .FirstOrDefault(t => t.Id == id)
               ?? throw new ArgumentException("Team not found");
    }    

    public bool AddUser(User user, Team team)
    {
        if (user.Equals(null) || team.Equals(null)) return false;
        var existingUserTeam = _dbContext.UserTeams
            .FirstOrDefault(ut => ut.UserId == user.Id && ut.TeamId == team.Id);

        if (existingUserTeam != null) return false;
        var userTeam = new UserTeam
        {
            UserId = user.Id,
            TeamId = team.Id
        };

        _dbContext.UserTeams.Add(userTeam);
        _dbContext.SaveChanges();
        return true;
    }
}