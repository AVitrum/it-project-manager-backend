using DatabaseService.Data.Models;

namespace Server.Payload.Responses;

public class UserCompanyResponse
{
    public required long Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }

    public static UserCompanyResponse ConvertToResponse(UserCompany userCompany)
    {
        return new UserCompanyResponse
        {
            Id = userCompany.User!.Id,
            Username = userCompany.User.Username,
            Email = userCompany.User.Email,
        };
    }
}