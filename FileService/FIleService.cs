using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FileService
{
    public class FileService(IConfiguration configuration) : IFileService
    {
        public async Task<string> UploadAsync(IFormFile file)
        {
            GetFirebaseAuth(out var apiKey, out var bucket, out var authEmail, out var authPassword);

            var storage = await GetFireBaseStorage(apiKey, authEmail, authPassword, bucket);

            var cancellation = new CancellationTokenSource();

            if (file.Length <= 0)
            {
                throw new FileException("No content in the file");
            }

            var stream = file.OpenReadStream();

            var link = await storage
                .Child("images")
                .Child(file.FileName)
                .PutAsync(stream, cancellation.Token);
            
            return link;
        }

        public async Task<string> UploadAsync(string name, IFormFile file)
        {
            GetFirebaseAuth(out var apiKey, out var bucket, out var authEmail, out var authPassword);

            var storage = await GetFireBaseStorage(apiKey, authEmail, authPassword, bucket);

            var cancellation = new CancellationTokenSource();

            if (file.Length <= 0) throw new FileException("No content in the file");

            var stream = file.OpenReadStream();

            
            var link = await storage
                .Child("images")
                .Child($"{name}_{file.FileName}")
                .PutAsync(stream, cancellation.Token);
            
            return link;
        }

        public async Task<string> DownloadAsync(string fileName)
        {
            GetFirebaseAuth(out var apiKey, out var bucket, out var authEmail, out var authPassword);

            var storage = await GetFireBaseStorage(apiKey, authEmail, authPassword, bucket);

            var downloadUrl = await storage
                .Child("images")
                .Child(fileName)
                .GetDownloadUrlAsync();

            return downloadUrl;
        }

        public void CheckImage(IFormFile file)
        {
            if (file.Length > 5e+6)
                throw new FileToLargeException();

            var imageFormats = new HashSet<string>
            {
                "image/jpeg",
                "image/png",
                "image/gif",
                "image/bmp",
            };

            if (!imageFormats.Contains(file.ContentType))
                throw new FileInvalidFormatException("Unsupported picture format!");
        }

        public async Task DeleteAsync(string fileName)
        {
            GetFirebaseAuth(out var apiKey, out var bucket, out var authEmail, out var authPassword);

            var storage = await GetFireBaseStorage(apiKey, authEmail, authPassword, bucket);

            await storage
                .Child("images")
                .Child(fileName)
                .DeleteAsync();
        }

        private static async Task<FirebaseStorage> GetFireBaseStorage(string? apiKey, string? authEmail, string? authPassword, string? bucket)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(authEmail, authPassword);
            
            var storage = new FirebaseStorage(
                bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                });
            return storage;
        }

        private void GetFirebaseAuth(out string? apiKey, out string? bucket, out string? authEmail, out string? authPassword)
        {
            apiKey = configuration["GoogleCloudStorage:ApiKey"];
            bucket = configuration["GoogleCloudStorage:Bucket"];
            authEmail = configuration["GoogleCloudStorage:AuthEmail"];
            authPassword = configuration["GoogleCloudStorage:AuthPassword"];
        }
    }
}
