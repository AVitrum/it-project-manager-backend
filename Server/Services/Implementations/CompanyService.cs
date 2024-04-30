using Server.Data.Enums;
using Server.Data.Models;
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

    public async Task CreatePositionAsync(long companyId, CreatePositionRequest request)
    {
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
            DeleteUser = (position.Permissions & PositionPermissions.DeleteUser) != 0
        };
    }

    public async Task AddUserAsync(long companyId, AddUserToCompanyRequest request)
    {
        var company = await companyRepository.GetByIdAsync(companyId);
        var userToAdd = await userRepository.GetByEmailAsync(request.Email);

        var currentUser = await userRepository.GetByJwtAsync();
        var employee = await companyRepository.GetByUserAndCompanyAsync(currentUser, company);

        if ((employee.PositionInCompany?.Permissions & PositionPermissions.AddUser) == 0)
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
}