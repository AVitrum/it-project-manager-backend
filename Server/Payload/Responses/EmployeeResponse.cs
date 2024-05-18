using DatabaseService.Data.DTOs;
using DatabaseService.Data.Enums;
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
    public required PositionInCompanyDto Permissions { get; set; }

    public static EmployeeResponse ConvertToResponse(Employee employee)
    {
        var permissions = new PositionInCompanyDto
        {
            Name = employee.PositionInCompany.Name,
            Priority = employee.PositionInCompany.Priority
        };
        
        var properties = typeof(PositionInCompanyDto).GetProperties()
            .Where(prop => prop.Name != "Name" && prop.Name != "Priority");
        
        foreach (var prop in properties)
        {
            var permissionFlag = (PositionPermissions)Enum.Parse(typeof(PositionPermissions), prop.Name);
            prop.SetValue(permissions, (employee.PositionInCompany.Permissions & permissionFlag) != 0);
        }
        
        return new EmployeeResponse
        {
            Id = employee.Id,
            Username = employee.User!.Username,
            Email = employee.User.Email,
            Position = employee.PositionInCompany!.Name,
            Salary = employee.Salary,
            Picture = employee.User.ProfilePhoto?.PictureLink,
            Permissions = permissions
        };
    }
}