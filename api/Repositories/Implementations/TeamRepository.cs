using api.Config;
using api.Data.Models;
using api.Data.SubModels;
using api.Exceptions;
using api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories.Implementations;

public class TeamRepository(AppDbContext dbContext) : ITeamRepository
{
    public async Task CreateAsync(Team team)
    {
        await dbContext.Teams.AddAsync(team);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Team team)
    {
        dbContext.Teams.Update(team);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Team team)
    {
        dbContext.Teams.Remove(team);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Team> GetAsync(long id)
    {
        return await dbContext.Teams
                   .Include(e => e.UserTeams)
                   .ThenInclude(e => e.User)
                   .ThenInclude(e => e.AdditionalInfo)
                   .FirstOrDefaultAsync(e => e.Id == id)
               ?? throw new EntityNotFoundException(new Team().GetType().Name);
    }

    public async Task<UserTeam?> FindByUserAndTeamAsync(User user, Team team)
    {
        var userTeam = await dbContext.UserTeams
            .FirstOrDefaultAsync(e => e.UserId == user.Id && e.TeamId == team.Id);
        return userTeam;
    }

    public async Task SaveUserInTeamAsync(UserTeam userTeam)
    {
        await dbContext.UserTeams.AddAsync(userTeam);
        await dbContext.SaveChangesAsync();
    }
}