namespace UserHelper.Responses;

public class UserInfoResponse
{
    public required long Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required DateTime CreationDate { get; set; }
}