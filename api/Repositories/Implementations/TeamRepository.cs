using api.Config;
using api.Data.Models;
using api.Data.SubModels;
using api.Exceptions;
using api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories.Implementations;

public class TeamRepository(AppDbContext dbContext) : ITeamRepository
{
    public void Create(Team team)
    {
        dbContext.Teams.Add(team);
        dbContext.SaveChanges();
    }

    public void Update(Team team)
    {
        throw new NotImplementedException();
    }

    public bool Delete(Team team)
    {
        throw new NotImplementedException();
    }

    public Team GetById(long id)
    {
        return dbContext.Teams
                   .Include(e => e.UserTeams) 
                   .ThenInclude(e => e.User)
                   .ThenInclude(e => e.AdditionalInfo)
                   .FirstOrDefault(e => e.Id == id)
               ?? throw new EntityNotFoundException(new Team().GetType().Name);
    }
    
    public UserTeam? FindByUserAndTeam(User user, Team team)
    {
        var userTeam = dbContext.UserTeams
            .FirstOrDefault(e => e.UserId == user.Id && e.TeamId == team.Id);
        return userTeam;
    }

    public void SaveUserInTeam(UserTeam userTeam)
    {
        dbContext.UserTeams.Add(userTeam);
        dbContext.SaveChanges();
    }
}