using Microsoft.AspNetCore.Http;

namespace FileService;

public interface IFileService
{
    Task<string> UploadAsync(IFormFile file);
    Task<string> UploadAsync(string username, IFormFile file);
    Task<string> DownloadAsync(string fileName);
    Task DeleteAsync(string fileName);
}