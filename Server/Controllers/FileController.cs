using FileService;
using Microsoft.AspNetCore.Mvc;
namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController(IWebHostEnvironment environment, IFileService fileService) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
    {
        await fileService.Upload(file);
        return Ok("Uploaded");
    }
 
    [HttpGet("download")]
    public async Task<IActionResult> Download(DownloadRequest request)
    {
        var downloadUrl = await fileService.Download(request.FileName);
        Console.WriteLine(downloadUrl);
        return Redirect(downloadUrl);
    }
}