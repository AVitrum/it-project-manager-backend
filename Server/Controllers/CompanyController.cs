using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Data.Requests;
using Server.Services.Interfaces;

namespace Server.Controllers;

[Route("server/[controller]")]
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

    [HttpPost("{teamId:long}/{userId:long}")]
    public async Task<IActionResult> AddUser(long teamId, long userId)
    {
        await companyService.AddUserAsync(teamId, userId);
        return Ok("Added");
    }

    [HttpGet("{teamId:long}")]
    public async Task<IActionResult> GetById(long teamId)
    {
        return Ok(await companyService.GetAsync(teamId));
    }
}