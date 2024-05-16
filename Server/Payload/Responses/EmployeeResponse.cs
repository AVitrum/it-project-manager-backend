using DatabaseService.Data.Models;

namespace Server.Payload.Responses;

public class EmployeeResponse
{
    public required long Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Position { get; set; }
    public required double Salary { get; set; }
    public string? Picture { get; set; }

    public static EmployeeResponse ConvertToResponse(Employee employee)
    {
        return new EmployeeResponse
        {
            Id = employee.Id,
            Username = employee.User!.Username,
            Email = employee.User.Email,
            Position = employee.PositionInCompany!.Name,
            Salary = employee.Salary,
            Picture = employee.User.ProfilePhoto?.PictureLink
        };
    }
}