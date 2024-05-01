using Server.Payload.Requests;
using Server.Payload.Responses;

namespace Server.Services.Interfaces;

public interface ICompanyService
{
    Task CreateAsync(CompanyCreationRequest request);
    Task<CompanyResponse> GetAsync(long id);
    
    Task CreatePositionAsync(long companyId, CreatePositionRequest request);
    Task<PositionPermissionsResponse> GetPositionAsync(long companyId, long positionId);
    
    Task AddUserAsync(long companyId, AddUserToCompanyRequest request);
    
    Task UpdateBudget(double budget, long companyId);
}