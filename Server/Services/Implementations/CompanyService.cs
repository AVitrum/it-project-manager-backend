using Server.Data.Enums;
using Server.Data.Models;
using Server.Data.Requests;
using Server.Data.Responses;
using Server.Repositories.Interfaces;
using Server.Services.Interfaces;

namespace Server.Services.Implementations;

public class CompanyService(ICompanyRepository companyRepository, IUserRepository userRepository) : ICompanyService
{
    public async Task CreateAsync(CompanyCreationRequest request)
    {
        var team = new Company
        {
            Name = request.Name,
            RegistrationDate = DateTime.UtcNow
        };

        await companyRepository.CreateAsync(team);

        var user = await userRepository.GetByCurrentTokenAsync();

        var userTeam = new UserCompany
        {
            UserId = user.Id,
            CompanyId = team.Id,
            Role = EmployeeRole.Manager
        };

        await companyRepository.SaveUserInCompanyAsync(userTeam);
    }

    public async Task AddUserAsync(long companyId, long userId)
    {
        var team = await companyRepository.GetByIdAsync(companyId);
        var user = await userRepository.GetByIdAsync(userId);

        if (await HasPermissionAsync(await userRepository.GetByCurrentTokenAsync(), team))
        {
            throw new Exception("Server error.");
        }

        if (await InCompanyAsync(user, team))
        {
            throw new ArgumentException("User already in this company");
        }

        var userTeam = new UserCompany
        {
            UserId = user.Id,
            CompanyId = team.Id,
            Role = EmployeeRole.Regular
        };

        await companyRepository.SaveUserInCompanyAsync(userTeam);
    }

    public async Task<CompanyResponse> GetAsync(long id)
    {
        var company = await companyRepository.GetByIdAsync(id);
        
        return new CompanyResponse
        {
            Id = company.Id,
            Name = company.Name,
            Users = company.UserCompanies
                .Select(UserCompanyResponse.ConvertToResponse).ToList()
        };
    }

    private async Task<bool> HasPermissionAsync(User user, Company company)
    {
        var userTeam = await companyRepository.FindByUserAndCompanyAsync(user, company);
        return userTeam is { Role: EmployeeRole.Manager };
    }

    private async Task<bool> InCompanyAsync(User user, Company company)
    {
        var userTeam = await companyRepository.FindByUserAndCompanyAsync(user, company);
        return userTeam != null;
    }
}