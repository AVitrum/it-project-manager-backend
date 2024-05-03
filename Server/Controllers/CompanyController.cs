using DatabaseService.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Payload.DTOs;
using Server.Payload.Requests;
using Server.Services.Interfaces;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CompanyController(ICompanyService companyService, IEmployeeService employeeService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(CompanyDto request)
    {
        await companyService.CreateAsync(request);
        return Ok(new
        {
            message = "Created"
        });
    }

    [HttpPut("{companyId:long}/updateCompany")]
    public async Task<IActionResult> UpdateCompany(long companyId, CompanyDto request)
    {
        await companyService.UpdateCompany(companyId, request);
        return Ok(new
        {
            message = "Performed"
        });
    }

    [HttpGet("{companyId:long}")]
    public async Task<IActionResult> GetById(long companyId)
    {
        return Ok(await companyService.GetAsync(companyId));
    }

    [HttpPost("{companyId:long}/addEmployee")]
    public async Task<IActionResult> AddUser(long companyId, EmployeeDto request)
    {
        await employeeService.AddEmployeeAsync(companyId, request);
        return Ok(new 
        {
            message = "Added"
        });
    }

    [HttpPut("{companyId:long}/updateEmployee")]
    public async Task<IActionResult> UpdateUser(long companyId, EmployeeDto request)
    {
        await employeeService.UpdateEmployeeAsync(companyId, request);
        return Ok(new
        {
            message = "Updated"
        });
    }

    [HttpDelete("{companyId:long}/removeEmployee")]
    public async Task<IActionResult> RemoveUser(long companyId, EmployeeDto request)
    {
        await employeeService.RemoveEmployeeAsync(companyId, request);
        return Ok(new
        {
            message = "Removed"
        });
    }

    [HttpPost("{companyId:long}/addPosition")]
    public async Task<IActionResult> AddPosition(long companyId, PositionInCompanyDto inCompanyDto)
    {
        await companyService.CreatePositionAsync(companyId, inCompanyDto);
        return Ok(new
        {
            message = "Created"
        });
    }
    
    [HttpPut("{companyId:long}/updatePosition")]
    public async Task<IActionResult> UpdatePosition(long companyId, PositionInCompanyDto inCompanyDto)
    {
        await companyService.UpdatePositionAsync(companyId, inCompanyDto);
        return Ok(new
        {
            message = "Updated"
        });
    }

    [HttpGet("{companyId:long}/{positionId:long}")]
    public async Task<IActionResult> GetPositionById(long companyId, long positionId)
    {
        return Ok(await employeeService.GetEmployeePositionAsync(companyId, positionId));
    }
}