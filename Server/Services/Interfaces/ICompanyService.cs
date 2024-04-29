using Server.Data.Models;
using Server.Payload.Requests;
using Server.Payload.Responses;

namespace Server.Services.Interfaces;

public interface ICompanyService
{
    Task CreateAsync(CompanyCreationRequest request);
    Task CreatePositionAsync(long companyId, CreatePositionRequest request);
    Task AddUserAsync(long companyId, AddUserToCompanyRequest request);
    Task<CompanyResponse> GetAsync(long id);
    Task<PositionPermissionsResponse> GetPositionAsync(long companyId, long positionId);
}