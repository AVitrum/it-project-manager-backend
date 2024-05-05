namespace Server.Payload.Responses;

public class ProjectResponse
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required double Budget { get; set; }
    public required List<EmployeeResponse> Performers { get; set; }
}