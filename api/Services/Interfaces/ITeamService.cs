using api.Data.Requests;
using api.Data.Responses;

namespace api.Services.Interfaces;

public interface ITeamService
{
    Task CreateAsync(TeamCreationRequest request);
    Task AddUserAsync(long teamId, long userId);
    Task<TeamResponse> GetAsync(long id);
}