using DatabaseService.Data.DTOs;
using DatabaseService.Data.Enums;
using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using Server.Payload.Requests;
using Server.Payload.Responses;
using Server.Services.Interfaces;

namespace Server.Services.Implementations;

public class CompanyService(
    ICompanyRepository companyRepository,
    IUserRepository userRepository)
    : ICompanyService
{
    public async Task CreateAsync(CompanyDto companyDto)
    {
        var newCompany = new Company
        {
            Name = companyDto.Name!,
            RegistrationDate = DateTime.UtcNow
        };
        var company = await companyRepository.CreateAsync(newCompany);

        var user = await userRepository.GetByJwtAsync();

        var newPositionInCompany = new PositionInCompany
        {
            CompanyId = company.Id,
            Name = "CEO",
            Priority = 0,
        };
        newPositionInCompany.AddAllPermissions();
        var positionInCompany = await companyRepository.CreatePositionAsync(newPositionInCompany);

        var newUserCompany = new UserCompany
        {
            UserId = user.Id,
            CompanyId = newCompany.Id,
            PositionInCompanyId = positionInCompany.Id,
        };

        await companyRepository.SaveUserInCompanyAsync(newUserCompany);
    }

    public async Task UpdateCompany(long companyId, CompanyDto companyDto)
    {
        var company = await companyRepository.GetByIdAsync(companyId);

        var performer = await companyRepository.GetEmployeeByUserAndCompanyAsync(
            await userRepository.GetByJwtAsync(), company);
        
        if (!performer.PositionInCompany!.HasPermissions(PositionPermissions.UpdateProject))
        {
            throw new PermissionException("You do not have permission to perform this action");
        }

        if (companyDto.Name != null)
        {
            company.Name = companyDto.Name;
        }
        
        if (companyDto.Description != null)
        {
            company.Description = companyDto.Description;
        }

        if (companyDto.Budget != null)
        {
            if ((!performer.PositionInCompany!.HasPermissions(PositionPermissions.AddBudget) || company.Budget != 0) 
                && (!performer.PositionInCompany!.HasPermissions(PositionPermissions.UpdateBudget) || !(company.Budget > 0)))
            {
                throw new PermissionException("You do not have permission to perform this action");
            }
            
            company.Budget = (double)companyDto.Budget;
        }
        
        await companyRepository.UpdateAsync(company);
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

    public async Task CreatePositionAsync(long companyId, PositionInCompanyDto positionInCompanyDto)
    {
        if (await companyRepository.PositionExistsByNameAndCompanyIdAsync(positionInCompanyDto.Name, companyId))
            throw new CompanyException("Position exists by name!");

        var performer = await companyRepository.GetEmployeeByUserAndCompanyAsync(
            await userRepository.GetByJwtAsync(), await companyRepository.GetByIdAsync(companyId));
        
        if (!performer.PositionInCompany!.HasPermissions(PositionPermissions.CreatePosition))
            throw new PermissionException("You do not have permission to perform this action");
        
        var position = new PositionInCompany
        {
            CompanyId = companyId,
            Name = positionInCompanyDto.Name,
            Priority = positionInCompanyDto.Priority,
        };
        position.SetPermissions(positionInCompanyDto);
        await companyRepository.CreatePositionAsync(position);
    }

    public async Task UpdatePositionAsync(long companyId, PositionInCompanyDto inCompanyDto)
    {
        if (!await companyRepository.PositionExistsByNameAndCompanyIdAsync(inCompanyDto.Name, companyId))
            throw new EntityNotFoundException(nameof(PositionInCompany));
        
        var performer = await companyRepository.GetEmployeeByUserAndCompanyAsync(
            await userRepository.GetByJwtAsync(), await companyRepository.GetByIdAsync(companyId));

        var position = await companyRepository.GetPositionByNameAndCompanyIdAsync(inCompanyDto.Name, companyId);
        
        if (!performer.PositionInCompany!.HasPermissions(PositionPermissions.UpdatePosition) ||
            (performer.PositionInCompany.Name == position.Name && position.Name != "CEO") ||
            (performer.PositionInCompany.Priority >= position.Priority && position.Name != "CEO"))
        {
            throw new PermissionException("You do not have permission to perform this action");
        }
        
        position.SetPermissions(inCompanyDto);
        await companyRepository.UpdatePositionAsync(position);
    }
}