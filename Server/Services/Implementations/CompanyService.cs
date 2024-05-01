using DatabaseService.Data.Enums;
using DatabaseService.Data.Models;
using DatabaseService.Repositories.Interfaces;
using Server.Payload.Requests;
using Server.Payload.Responses;
using Server.Repositories.Interfaces;
using Server.Services.Interfaces;

namespace Server.Services.Implementations;

public class CompanyService(
    ICompanyRepository companyRepository,
    IUserRepository userRepository) 
    : ICompanyService
{
    public async Task CreateAsync(CompanyCreationRequest request)
    {
        var newCompany = new Company
        {
            Name = request.Name,
            RegistrationDate = DateTime.UtcNow
        };
        var company = await companyRepository.CreateAsync(newCompany);

        var user = await userRepository.GetByJwtAsync();

        var positionPermissions = PositionPermissions.None;
        PositionPermissionsHelper.AddAllPermissions(ref positionPermissions);
        var newPositionInCompany = new PositionInCompany
        {
            CompanyId = company.Id,
            Name = "CEO",
            Permissions = positionPermissions
        };
        var positionInCompany = await companyRepository.CreatePositionAsync(newPositionInCompany);

        var newUserCompany = new UserCompany
        {
            UserId = user.Id,
            CompanyId = newCompany.Id,
            PositionInCompanyId = positionInCompany.Id,
        };

        await companyRepository.SaveUserInCompanyAsync(newUserCompany);
    }

    public async Task<CompanyResponse> GetAsync(long id)
    {
        var company = await companyRepository.GetByIdAsync(id);
        
        return new CompanyResponse
        {
            Id = company.Id,
            Name = company.Name,
            Users = company.UserCompanies!
                .Select(UserCompanyResponse.ConvertToResponse).ToList()
        };
    }

    public async Task AddUserAsync(long companyId, AddUserToCompanyRequest request)
    {
        var company = await companyRepository.GetByIdAsync(companyId);
        var userToAdd = await userRepository.GetByEmailAsync(request.Email);

        var currentUser = await userRepository.GetByJwtAsync();
        var employee = await companyRepository.GetUserCompanyByUserAndCompanyAsync(currentUser, company);

        if (!PositionPermissionsHelper.HasPermissions(
                employee.PositionInCompany!.Permissions, PositionPermissions.AddUser))
            throw new Exception("You don't have this permission");

        if (await companyRepository.ExistsByUserAndCompanyAsync(userToAdd, company))
            throw new ArgumentException("User already in this company");
        
        var position = await companyRepository.GetPositionByNameAndCompanyIdAsync(request.PositionName, companyId);

        var userTeam = new UserCompany
        {
            UserId = userToAdd.Id,
            CompanyId = company.Id,
            PositionInCompanyId = position.Id,
        };

        await companyRepository.SaveUserInCompanyAsync(userTeam);
    }

    public async Task UpdateBudget(double budget, long companyId)
    {
        var company = await companyRepository.GetByIdAsync(companyId);

        var performer = await companyRepository.GetUserCompanyByUserAndCompanyAsync(
            await userRepository.GetByJwtAsync(), company);

        if ((!PositionPermissionsHelper.HasPermissions(performer.PositionInCompany!.Permissions,
                PositionPermissions.AddBudget) || company.Budget != 0)
            && (!PositionPermissionsHelper.HasPermissions(performer.PositionInCompany!.Permissions,
                PositionPermissions.UpdateBudget) || !(company.Budget > 0)))
        {
            throw new ArgumentException("You do not have permission to perform this action");
        }


        company.Budget = budget;
        await companyRepository.UpdateAsync(company);
    }

    public async Task CreatePositionAsync(long companyId, CreatePositionRequest request)
    {

        if (await companyRepository.PositionExistsByNameAndCompanyIdAsync(request.Name, companyId))
        {
            throw new ArgumentException("Position exists by name!");
        }
        
        var position = new PositionInCompany
        {
            CompanyId = companyId,
            Name = request.Name,
            Permissions = (request.CreateProject ? PositionPermissions.CreateProject : PositionPermissions.None) |
                          (request.UpdateProject ? PositionPermissions.UpdateProject : PositionPermissions.None) |
                          (request.DeleteProject ? PositionPermissions.DeleteProject : PositionPermissions.None) |
                          (request.AddUser ? PositionPermissions.AddUser : PositionPermissions.None) |
                          (request.DeleteUser ? PositionPermissions.DeleteUser : PositionPermissions.None)
        };

        await companyRepository.CreatePositionAsync(position);
    }

    public async Task<PositionPermissionsResponse> GetPositionAsync(long companyId, long positionId)
    {
        var position = await companyRepository.GetPositionByIdAndCompanyIdAsync(positionId, companyId);

        return new PositionPermissionsResponse
        {
            PositionName = position.Name,
            CreateProject = (position.Permissions & PositionPermissions.CreateProject) != 0,
            UpdateProject = (position.Permissions & PositionPermissions.UpdateProject) != 0,
            DeleteProject = (position.Permissions & PositionPermissions.DeleteProject) != 0,
            AddUser = (position.Permissions & PositionPermissions.AddUser) != 0,
            DeleteUser = (position.Permissions & PositionPermissions.DeleteUser) != 0,
            AddBudget = (position.Permissions & PositionPermissions.AddBudget) != 0,
            UpdateBudget = (position.Permissions & PositionPermissions.UpdateBudget) != 0,
        };
    }
}