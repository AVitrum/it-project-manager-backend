using api.Data.Models;
using api.Data.SubModels;

namespace api.Repositories.Interfaces;

public interface ITeamRepository
{
    Task CreateAsync(Team team);
    Task UpdateAsync(Team team);
    Task<bool> DeleteAsync(Team team);
    Task<Team> GetAsync(long id);

    Task SaveUserInTeamAsync(UserTeam userTeam);
    Task<UserTeam?> FindByUserAndTeamAsync(User user, Team team);
}