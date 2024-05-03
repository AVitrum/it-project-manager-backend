using DatabaseService.Data.DTOs;
using Server.Payload.DTOs;
using Server.Payload.Requests;
using Server.Payload.Responses;

namespace Server.Services.Interfaces;

public interface ICompanyService
{
    Task CreateAsync(CompanyCreationRequest request);
    Task<CompanyResponse> GetAsync(long id);
    Task CreatePositionAsync(long companyId, PositionInCompanyDto positionInCompanyDto);
    Task UpdatePositionAsync(long companyId, PositionInCompanyDto inCompanyDto);
    Task UpdateBudget(double budget, long companyId);
}