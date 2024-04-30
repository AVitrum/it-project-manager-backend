namespace Server.Payload.Requests;

public class CreatePositionRequest
{
    public required string Name { get; set; }
    public bool CreateProject { get; set; } = false;
    public bool UpdateProject { get; set; } = false;
    public bool DeleteProject { get; set; } = false;
    public bool AddUser { get; set; } = false;
    public bool DeleteUser { get; set; } = false;
}