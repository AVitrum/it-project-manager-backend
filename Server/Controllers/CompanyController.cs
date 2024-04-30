using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Payload.Requests;
using Server.Services.Interfaces;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CompanyController(ICompanyService companyService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(CompanyCreationRequest request)
    {
        await companyService.CreateAsync(request);
        return Ok("Created");
    }

    [HttpGet("{companyId:long}")]
    public async Task<IActionResult> GetById(long companyId)
    {
        return Ok(await companyService.GetAsync(companyId));
    }

    [HttpPost("{companyId:long}/addUser")]
    public async Task<IActionResult> AddUser(long companyId, AddUserToCompanyRequest request)
    {
        await companyService.AddUserAsync(companyId, request);
        return Ok("Added");
    }

    [HttpPost("{companyId:long}/addPosition")]
    public async Task<IActionResult> AddPosition(long companyId, CreatePositionRequest request)
    {
        await companyService.CreatePositionAsync(companyId, request);
        return Ok("Created");
    }

    [HttpGet("{companyId:long}/{positionId:long}")]
    public async Task<IActionResult> GetPositionById(long companyId, long positionId)
    {
        return Ok(await companyService.GetPositionAsync(companyId, positionId));
    }
}