using Microsoft.AspNetCore.Http;

namespace FileService;

public interface IFileService
{
    Task<string> UploadAsync(IFormFile file);
    Task<string> UploadAsync(string name, IFormFile file);
    Task<(string, string)> UploadFileAsync(string folder, IFormFile file);
    Task<string> DownloadAsync(string fileName, string folder);

    void CheckImage(IFormFile file);
    
    Task DeleteAsync(string fileName);
}