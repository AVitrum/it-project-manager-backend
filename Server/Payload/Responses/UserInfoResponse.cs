namespace Server.Payload.Responses;

public class UserInfoResponse
{
    public required long Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required DateTime CreationDate { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ImageUrl { get; set; }
    public double AverageSalary { get; set; }
}