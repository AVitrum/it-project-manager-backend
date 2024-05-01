namespace Server.Payload.Responses;

public class PositionPermissionsResponse
{
    public required string PositionName { get; set; }
    public required bool CreateProject { get; set; } = false;
    public required bool UpdateProject { get; set; } = false;
    public required bool DeleteProject { get; set; } = false;
    public required bool AddUser { get; set; } = false;
    public required bool DeleteUser { get; set; } = false;
    public required bool AddBudget { get; set; } = false;
    public required bool UpdateBudget { get; set; } = false;
}