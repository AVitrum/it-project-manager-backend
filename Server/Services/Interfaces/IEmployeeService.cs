using Server.Payload.DTOs;
using Server.Payload.Responses;

namespace Server.Services.Interfaces;

public interface IEmployeeService
{
    Task AddEmployeeAsync(long companyId, EmployeeDto employeeDto);
    Task RemoveEmployeeAsync(long companyId, EmployeeDto employeeDto);
    Task<PositionPermissionsResponse> GetEmployeePositionAsync(long companyId, long positionId);
    Task UpdateEmployeeAsync(long companyId, EmployeeDto employeeDto);
}