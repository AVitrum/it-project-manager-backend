using DatabaseService.Data.DTOs;
using DatabaseService.Data.Enums;
using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using FileService;
using Server.Payload.Requests;
using Server.Payload.Responses;
using Server.Services.Interfaces;

namespace Server.Services.Implementations;

public class CompanyService(
    ICompanyRepository companyRepository,
    IUserRepository userRepository,
    IProjectRepository projectRepository,
    IFileService fileService)
    : ICompanyService
{
    public async Task CreateAsync(CompanyDto companyDto)
    {
        if (await companyRepository.ExistsByNameAsync(companyDto.Name!.Trim()))
        {
            throw new DatabaseException("Company exists by name");
        }
        
        var newCompany = new Company
        {
            Name = companyDto.Name!,
            Description = companyDto.Description!,
            Budget = (double)companyDto.Budget!,
            RegistrationDate = DateTime.UtcNow,
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

        var newPositionInCompany2 = new PositionInCompany
        {
            CompanyId = company.Id,
            Name = "Default",
            Priority = int.MaxValue,
        };
        await companyRepository.CreatePositionAsync(newPositionInCompany2);

        var newEmployee = new Employee
        {
            UserId = user.Id,
            CompanyId = newCompany.Id,
            PositionInCompanyId = positionInCompany.Id,
            PositionInCompany = newPositionInCompany
        };

        await companyRepository.SaveUserInCompanyAsync(newEmployee);
    }

    public async Task UpdateCompany(long companyId, CompanyDto companyDto)
    {
        var company = await companyRepository.GetByIdAsync(companyId);

        if (await companyRepository.ExistsByNameAsync(companyDto.Name!.Trim()) && companyDto.Name != company.Name)
        {
            throw new DatabaseException("Company exists by name");
        }
        
        var performer = await companyRepository.GetEmployeeByUserAndCompanyAsync(
            await userRepository.GetByJwtAsync(), company);
        
        if (!performer.PositionInCompany.HasPermissions(PositionPermissions.UpdateProject))
        {
            throw new PermissionException();
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
            if ((!performer.PositionInCompany.HasPermissions(PositionPermissions.AddBudget) || company.Budget != 0) 
                && (!performer.PositionInCompany.HasPermissions(PositionPermissions.UpdateBudget) || !(company.Budget > 0)))
            {
                throw new PermissionException();
            }
            
            company.Budget = (double)companyDto.Budget;
        }
        
        await companyRepository.UpdateAsync(company);
    }

    public async Task ChangeCompanyImage(long companyId, IFormFile file)
    {
        var company = await companyRepository.GetByIdAsync(companyId);

        var performer = await companyRepository.GetEmployeeByUserAndCompanyAsync(
            await userRepository.GetByJwtAsync(), company);

        if (!performer.PositionInCompany.HasPermissions(PositionPermissions.UpdateProject))
        {
            throw new PermissionException();
        }
        
        fileService.CheckImage(file);
        
        if (company.PictureName != null)
            await fileService.DeleteAsync(company.PictureName);
        
        var imageUrl = await fileService.UploadAsync(company.Name, file);

        company.PictureLink = imageUrl;
        company.PictureName = $"{company.Name}_{file.FileName}";
        await companyRepository.UpdateAsync(company);
    }

    public async Task<CompanyResponse> GetAsync(long id)
    {
        var company = await companyRepository.GetByIdAsync(id);

        var employees = await companyRepository.GetAllEmployeesByCompany(company);
        var projects = await projectRepository.GetAllByCompanyAsync(company.Id);
        return new CompanyResponse
        {
            Id = company.Id,
            Name = company.Name,
            Picture = company.PictureLink,
            Employees = employees.Select(EmployeeResponse.ConvertToResponse)
                .ToList(),
            Projects = projects.Select(ProjectResponse.ConvertToResponse)
                .ToList(),
            Description = company.Description,
            Budget = company.Budget
        };
    }

    public async Task<List<CompanyResponse>> GetAllUserCompaniesAsync()
    {
        var companies = await companyRepository.GetAllByUserAsync(await userRepository.GetByJwtAsync());

        var responses = new List<CompanyResponse>();

        foreach (var company in companies)
        {
            var employees = await companyRepository.GetAllEmployeesByCompany(company);
            var projects = await projectRepository.GetAllByCompanyAsync(company.Id);
            responses.Add(new CompanyResponse
            {
                Id = company.Id,
                Name = company.Name,
                Picture = company.PictureLink,
                Employees = employees.Select(EmployeeResponse.ConvertToResponse)
                    .ToList(),
                Projects = projects.Select(ProjectResponse.ConvertToResponse)
                    .ToList(),
                Description = company.Description,
                Budget = company.Budget
            });
        }
        
        responses.Sort((a, b) => a.Id.CompareTo(b.Id));
        return responses;
    }

    public async Task CreatePositionAsync(long companyId, PositionInCompanyDto positionInCompanyDto)
    {
        if (await companyRepository.PositionExistsByNameAndCompanyIdAsync(positionInCompanyDto.Name, companyId))
            throw new CompanyException("Position exists by name!");

        var performer = await companyRepository.GetEmployeeByUserAndCompanyAsync(
            await userRepository.GetByJwtAsync(), await companyRepository.GetByIdAsync(companyId));
        
        if (!performer.PositionInCompany.HasPermissions(PositionPermissions.CreatePosition))
            throw new PermissionException();
        
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
        
        if (!performer.PositionInCompany.HasPermissions(PositionPermissions.UpdatePosition) ||
            (performer.PositionInCompany.Name == position.Name && position.Name != "CEO") ||
            (performer.PositionInCompany.Priority >= position.Priority && position.Name != "CEO"))
        {
            throw new PermissionException();
        }
        
        position.SetPermissions(inCompanyDto);
        await companyRepository.UpdatePositionAsync(position);
    }
}