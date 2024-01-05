using api.Data.Models;
using api.Data.Requests;

namespace api.Services.Interfaces;

public interface ITeamService
{
    void Create(TeamCreationRequest request);
    Team Get(long id);
    bool AddUser(User user, Team team);
}