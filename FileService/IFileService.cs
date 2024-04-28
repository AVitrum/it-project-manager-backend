using Microsoft.AspNetCore.Http;

namespace FileService;

public interface IFileService
{
    Task Upload(IFormFile file);
    Task<string> Download(string fileName);
}