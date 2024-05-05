using DatabaseService.Data.DTOs;
using Server.Payload.DTOs;

namespace Server.Services.Interfaces;

public interface IEmployeeService
{
    Task AddEmployeeAsync(long companyId, EmployeeDto employeeDto);
    Task RemoveEmployeeAsync(long companyId, EmployeeDto employeeDto);
    Task<PositionInCompanyDto> GetEmployeePositionAsync(long companyId, long positionId);
    Task UpdateEmployeeAsync(long companyId, EmployeeDto employeeDto);
}