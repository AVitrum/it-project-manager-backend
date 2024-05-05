using DatabaseService.Data.DTOs;
using DatabaseService.Data.Enums;
using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using Server.Payload.DTOs;
using EmailService;
using Server.Services.Interfaces;

namespace Server.Services.Implementations;

public class EmployeeService(
    IEmailSender emailSender,
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

        if (!employer.PositionInCompany.HasPermissions(PositionPermissions.AddUser))
            throw new PermissionException();

        if (await companyRepository.ExistsByUserAndCompanyAsync(userToAdd, company))
            throw new CompanyException("User already in this company");

        var position = await companyRepository.GetPositionByNameAndCompanyIdAsync(employeeDto.Position!, companyId);

        var userTeam = new Employee
        {
            UserId = userToAdd.Id,
            CompanyId = company.Id,
            PositionInCompanyId = position.Id,
            PositionInCompany = position,
        };

        await companyRepository.SaveUserInCompanyAsync(userTeam);

        await emailSender.SendEmailAsync(
            userToAdd.Email,
            company.Name,
            "You have been added to the company");
    }

    public async Task UpdateEmployeeAsync(long companyId, EmployeeDto employeeDto)
    {
        var company = await companyRepository.GetByIdAsync(companyId);
        var currentUser = await userRepository.GetByJwtAsync();
        var performer = await companyRepository.GetEmployeeByUserAndCompanyAsync(currentUser, company);
        
        if (!performer.PositionInCompany.HasPermissions(PositionPermissions.UpdateUser))
            throw new PermissionException();
        
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
        
        await emailSender.SendEmailAsync(
            userToUpdate.Email,
            company.Name,
            "Changes have been made to your position");
    }

    public async Task RemoveEmployeeAsync(long companyId, EmployeeDto employeeDto)
    {
        var company = await companyRepository.GetByIdAsync(companyId);
        var currentUser = await userRepository.GetByJwtAsync();
        var performer = await companyRepository.GetEmployeeByUserAndCompanyAsync(currentUser, company);
        
        if (!performer.PositionInCompany.HasPermissions(PositionPermissions.DeleteUser))
            throw new PermissionException();
        
        var userToRemove = await userRepository.GetByEmailAsync(employeeDto.Email!);
        var employee = await companyRepository.GetEmployeeByUserAndCompanyAsync(userToRemove, company);

        if (!await companyRepository.ExistsByUserAndCompanyAsync(userToRemove, company))
            throw new EntityNotFoundException("Employee");

        await companyRepository.RemoveUserFromCompanyAsync(employee);
        
        await emailSender.SendEmailAsync(
            userToRemove.Email,
            company.Name,
            "You have been removed from the company");
    }

    public async Task<PositionInCompanyDto> GetEmployeePositionAsync(long companyId, long positionId)
    {
        var position = await companyRepository.GetPositionByIdAndCompanyIdAsync(positionId, companyId);
        
        var permissions = new PositionInCompanyDto
        {
            Name = position.Name,
            Priority = position.Priority
        };

        var properties = typeof(PositionInCompanyDto).GetProperties()
            .Where(prop => prop.Name != "Name" && prop.Name != "Priority");

        foreach (var prop in properties)
        {
            var permissionFlag = (PositionPermissions)Enum.Parse(typeof(PositionPermissions), prop.Name);
            prop.SetValue(permissions, (position.Permissions & permissionFlag) != 0);
        }

        return permissions;
    }
}