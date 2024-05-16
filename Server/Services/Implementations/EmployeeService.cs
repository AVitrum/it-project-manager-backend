using DatabaseService.Data.DTOs;
using DatabaseService.Data.Enums;
using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using Server.Payload.DTOs;
using EmailService;
using Server.Payload.Responses;
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
        
        var userToUpdate = await userRepository.GetByEmailAsync(employeeDto.Email!);
        var employee = await companyRepository.GetEmployeeByUserAndCompanyAsync(userToUpdate, company);
        
        if (!performer.PositionInCompany.HasPermissions(PositionPermissions.UpdateUser))
            throw new PermissionException();

        if (employeeDto.Position != null)
        {
            var newPosition = await companyRepository
                .GetPositionByNameAndCompanyIdAsync(employeeDto.Position, companyId);
            
            if ((performer.PositionInCompany.Priority >= employee.PositionInCompany.Priority
                || performer.PositionInCompany.Priority >= newPosition.Priority) 
                && performer.PositionInCompany.Name != "CEO")
            {
                throw new PermissionException();
            }

            employee.PositionInCompany = newPosition;
        }

        if (employeeDto.Salary != null)
        {
            if (employeeDto.Salary >= company.Budget)
                throw new CompanyException("You don't have enough money!");
            employee.Salary = (double)employeeDto.Salary;
        }

        await companyRepository.UpdateEmployeeAsync(employee);
        
        await emailSender.SendEmailAsync(
            userToUpdate.Email,
            company.Name,
            "Changes have been made to your position");
    }

    public async Task<EmployeeResponse> GetEmployeeAsync(long employeeId)
    {
        var employee = await companyRepository.GetEmployeeById(employeeId);
        
        return EmployeeResponse.ConvertToResponse(employee);
    }

    public async Task<PositionInCompanyDto> GetEmployeePositionAsync(long companyId, long positionId)
    {
        var position = await companyRepository.GetPositionByIdAndCompanyIdAsync(positionId, companyId);
        
        var permissions = ConvertToPositionInCompanyDto(position);

        return permissions;
    }

    public async Task<List<PositionInCompanyDto>> GetAllPositionsAsync(long companyId)
    {
        var positions = await companyRepository.GetPositionsByCompanyIdAsync(companyId);

        return positions.Select(ConvertToPositionInCompanyDto).ToList();
    }

    private static PositionInCompanyDto ConvertToPositionInCompanyDto(PositionInCompany position)
    {
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

    public async Task RemoveEmployeeAsync(long companyId, EmployeeDto employeeDto)
    {
        var company = await companyRepository.GetByIdAsync(companyId);
        
        var currentUser = await userRepository.GetByJwtAsync();
        var performer = await companyRepository.GetEmployeeByUserAndCompanyAsync(currentUser, company);
        
        var userToRemove = await userRepository.GetByEmailAsync(employeeDto.Email!);
        var employee = await companyRepository.GetEmployeeByUserAndCompanyAsync(userToRemove, company);
        
        if (employee.PositionInCompany.Name == "CEO")
            throw new CompanyException("You can't leave the company");
        
        if ((!performer.PositionInCompany.HasPermissions(PositionPermissions.DeleteUser) && performer.Id != employee.Id) 
            || performer.PositionInCompany.Priority >= employee.PositionInCompany.Priority)
            throw new PermissionException();
        
        await companyRepository.RemoveUserFromCompanyAsync(employee);
        
        await emailSender.SendEmailAsync(
            userToRemove.Email,
            company.Name,
            "You have been removed from the company");
    }
}