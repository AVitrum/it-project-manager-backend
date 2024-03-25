using Server.Data.Requests;
using Server.Data.Responses;

namespace Server.Services.Interfaces;

public interface ICompanyService
{
    Task CreateAsync(CompanyCreationRequest request);
    Task AddUserAsync(long companyId, long userId);
    Task<CompanyResponse> GetAsync(long id);
}