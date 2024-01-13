using api.Data.Requests;
using api.Data.Responses;

namespace api.Services.Interfaces;

public interface ITeamService
{
    void Create(TeamCreationRequest request);
    void AddUser(long teamId, long userId);
    TeamResponse Get(long id);
}