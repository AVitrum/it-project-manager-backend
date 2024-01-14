using Server.Data.Requests;
using Server.Data.Responses;

namespace Server.Services.Interfaces;

public interface ITeamService
{
    Task CreateAsync(TeamCreationRequest request);
    Task AddUserAsync(long teamId, long userId);
    Task<TeamResponse> GetAsync(long id);
}