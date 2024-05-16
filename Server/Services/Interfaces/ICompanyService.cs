using DatabaseService.Data.DTOs;
using Server.Payload.Requests;
using Server.Payload.Responses;

namespace Server.Services.Interfaces;

public interface ICompanyService
{
    Task CreateAsync(CompanyDto companyDto);
    Task UpdateCompany(long companyId, CompanyDto companyDto);
    Task<CompanyResponse> GetAsync(long id);
    Task<List<CompanyResponse>> GetAllUserCompaniesAsync(string order);
    Task CreatePositionAsync(long companyId, PositionInCompanyDto positionInCompanyDto);
    Task UpdatePositionAsync(long companyId, PositionInCompanyDto inCompanyDto);
    Task ChangeCompanyImage(long companyId, IFormFile file);
}