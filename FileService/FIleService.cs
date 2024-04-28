using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FileService
{
    public class FileService(IConfiguration configuration) : IFileService
    {
        public async Task Upload(IFormFile file)
        {
            var apiKey = configuration["GoogleCloudStorage:ApiKey"];
            var bucket = configuration["GoogleCloudStorage:Bucket"];
            var authEmail = configuration["GoogleCloudStorage:AuthEmail"];
            var authPassword = configuration["GoogleCloudStorage:AuthPassword"];

            var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(authEmail, authPassword);

            var cancellation = new CancellationTokenSource();

            if (file.Length <= 0) throw new Exception("No content in the file");

            var stream = file.OpenReadStream();

            var task = new FirebaseStorage(
                    bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                .Child("images")
                .Child(file.FileName)
                .PutAsync(stream, cancellation.Token);
            var link = await task;
        }

        public async Task<string> Download(string fileName)
        {
            var apiKey = configuration["GoogleCloudStorage:ApiKey"];
            var bucket = configuration["GoogleCloudStorage:Bucket"];
            var authEmail = configuration["GoogleCloudStorage:AuthEmail"];
            var authPassword = configuration["GoogleCloudStorage:AuthPassword"];

            var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(authEmail, authPassword);

            var storage = new FirebaseStorage(
                bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                });

            var downloadUrl = await storage
                .Child("images")
                .Child(fileName)
                .GetDownloadUrlAsync();

            return downloadUrl;
        }
    }
}
