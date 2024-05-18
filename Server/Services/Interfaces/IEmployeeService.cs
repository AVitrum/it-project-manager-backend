using DatabaseService.Data.DTOs;
using Server.Payload.DTOs;
using Server.Payload.Responses;

namespace Server.Services.Interfaces;

public interface IEmployeeService
{
    Task AddEmployeeAsync(long companyId, EmployeeDto employeeDto);
    Task RemoveEmployeeAsync(long companyId, EmployeeDto employeeDto);
    Task<PositionInCompanyDto> GetEmployeePositionAsync(long companyId, long positionId);
    Task<List<PositionInCompanyDto>> GetAllPositionsAsync(long companyId);
    Task UpdateEmployeeAsync(long companyId, EmployeeDto employeeDto);
    Task<EmployeeResponse> GetEmployeeAsync(long employeeId);
    Task<EmployeeResponse> GetPerformerAsync(long companyId);
}