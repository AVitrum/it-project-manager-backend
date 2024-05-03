using DatabaseService.Data.Enums;
using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using Server.Payload.DTOs;
using Server.Payload.Responses;
using Server.Services.Interfaces;

namespace Server.Services.Implementations;

public class EmployeeService(
    ICompanyRepository companyRepository,
    IUserRepository userRepository) 
    : IEmployeeService
{
    public async Task AddEmployeeAsync(long companyId, EmployeeDto employeeDto)
    {
        var company = await companyRepository.GetByIdAsync(companyId);
        var userToAdd = await userRepository.GetByEmailAsync(employeeDto.Email!);

        var currentUser = await userRepository.GetByJwtAsync();
        var employer = await companyRepository.GetEmployeeByUserAndCompanyAsync(currentUser, company);

        if (!employer.PositionInCompany!.HasPermissions(PositionPermissions.AddUser))
            throw new PermissionException("You don't have this permission");

        if (await companyRepository.ExistsByUserAndCompanyAsync(userToAdd, company))
            throw new CompanyException("User already in this company");

        var position = await companyRepository.GetPositionByNameAndCompanyIdAsync(employeeDto.Position!, companyId);

        var userTeam = new UserCompany
        {
            UserId = userToAdd.Id,
            CompanyId = company.Id,
            PositionInCompanyId = position.Id,
        };

        await companyRepository.SaveUserInCompanyAsync(userTeam);
    }

    public async Task UpdateEmployeeAsync(long companyId, EmployeeDto employeeDto)
    {
        var company = await companyRepository.GetByIdAsync(companyId);
        var currentUser = await userRepository.GetByJwtAsync();
        var performer = await companyRepository.GetEmployeeByUserAndCompanyAsync(currentUser, company);
        
        if (!performer.PositionInCompany!.HasPermissions(PositionPermissions.UpdateUser))
            throw new PermissionException("You don't have this permission");
        
        var userToUpdate = await userRepository.GetByEmailAsync(employeeDto.Email!);
        var employee = await companyRepository.GetEmployeeByUserAndCompanyAsync(userToUpdate, company);

        if (employeeDto.Position != null)
        {
            employee.PositionInCompany = await companyRepository
                .GetPositionByNameAndCompanyIdAsync(employeeDto.Position, companyId);
        }

        if (employeeDto.Salary != null)
        {
            employee.Salary = (double)employeeDto.Salary;
        }

        await companyRepository.UpdateEmployeeAsync(employee);
    }

    public async Task RemoveEmployeeAsync(long companyId, EmployeeDto employeeDto)
    {
        var company = await companyRepository.GetByIdAsync(companyId);
        var currentUser = await userRepository.GetByJwtAsync();
        var performer = await companyRepository.GetEmployeeByUserAndCompanyAsync(currentUser, company);
        
        if (!performer.PositionInCompany!.HasPermissions(PositionPermissions.DeleteUser))
            throw new PermissionException("You don't have this permission");
        
        var userToRemove = await userRepository.GetByEmailAsync(employeeDto.Email!);
        var employee = await companyRepository.GetEmployeeByUserAndCompanyAsync(userToRemove, company);

        if (!await companyRepository.ExistsByUserAndCompanyAsync(userToRemove, company))
            throw new EntityNotFoundException("Employee");

        await companyRepository.RemoveUserFromCompanyAsync(employee);
    }

    public async Task<PositionPermissionsResponse> GetEmployeePositionAsync(long companyId, long positionId)
    {
        var position = await companyRepository.GetPositionByIdAndCompanyIdAsync(positionId, companyId);
        
        var permissions = new PositionPermissionsResponse
        {
            PositionName = position.Name
        };

        var properties = typeof(PositionPermissionsResponse).GetProperties()
            .Where(prop => prop.Name != "PositionName");

        foreach (var prop in properties)
        {
            var permissionFlag = (PositionPermissions)Enum.Parse(typeof(PositionPermissions), prop.Name);
            prop.SetValue(permissions, (position.Permissions & permissionFlag) != 0);
        }

        return permissions;
    }
}