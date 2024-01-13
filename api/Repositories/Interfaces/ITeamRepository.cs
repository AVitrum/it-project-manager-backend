using api.Data.Models;
using api.Data.SubModels;

namespace api.Repositories.Interfaces;

public interface ITeamRepository
{
    void Create(Team team);
    void Update(Team team);
    bool Delete(Team team);
    void SaveUserInTeam(UserTeam userTeam);
    Team GetById(long id);
    UserTeam? FindByUserAndTeam(User user, Team team);
}