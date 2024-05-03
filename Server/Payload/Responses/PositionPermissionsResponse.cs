namespace Server.Payload.Responses;

public class PositionPermissionsResponse
{
    public required string PositionName { get; set; }
    public bool CreateProject { get; set; } 
    public bool UpdateProject { get; set; } 
    public bool DeleteProject { get; set; } 
    public bool AddUser { get; set; } 
    public bool UpdateUser { get; set; }
    public bool DeleteUser { get; set; } 
    public bool AddBudget { get; set; } 
    public bool UpdateBudget { get; set; } 
    public bool CreatePosition { get; set; } 
    public bool UpdatePosition { get; set; } 
}