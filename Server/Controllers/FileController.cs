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
        var (link, fileName) = await fileService.UploadFileAsync("Company 1", file);
        Console.WriteLine(link);
        Console.WriteLine(fileName);
        return Ok("Uploaded");
    }
 
    [HttpGet("download")]
    public async Task<IActionResult> Download(DownloadRequest request)
    {
        var downloadUrl = await fileService.DownloadAsync(request.FileName, "images");
        Console.WriteLine(downloadUrl);
        return Redirect(downloadUrl);
    }
}