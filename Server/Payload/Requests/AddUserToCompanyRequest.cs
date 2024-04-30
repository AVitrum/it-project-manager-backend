namespace Server.Payload.Requests;

public class AddUserToCompanyRequest
{
    public required string PositionName { get; set; }
    public required string Email { get; set; }
}