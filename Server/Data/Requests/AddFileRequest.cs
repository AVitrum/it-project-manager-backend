namespace Server.Data.Requests;

public class AddFileRequest
{
    public required IFormFile File { get; set; }
}