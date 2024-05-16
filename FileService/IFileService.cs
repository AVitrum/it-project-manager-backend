using Microsoft.AspNetCore.Http;

namespace FileService;

public interface IFileService
{
    Task<string> UploadAsync(IFormFile file);
    Task<string> UploadAsync(string name, IFormFile file);
    Task<string> DownloadAsync(string fileName);

    void CheckImage(IFormFile file);
    
    Task DeleteAsync(string fileName);
}