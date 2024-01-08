using api.Data.Enums;
using api.Data.Models;
using api.Data.Requests;

namespace api.Services.Interfaces;

public interface ITeamService
{
    void Create(TeamCreationRequest request, User user);
    Team Get(long id);
    bool AddUser(User user, Team team, UserRole role);
    bool HasPermission(User user, Team team);
}