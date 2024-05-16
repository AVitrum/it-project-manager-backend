using DatabaseService.Data.Models;

namespace Server.Payload.Responses;

public class AssignmentResponse
{
    public required long Id { get; set; }
    public required string Theme { get; set; }
    public required string Description { get; set; }
    public required double Budget { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime Deadline { get; set; }
    public required List<EmployeeResponse> Performers { get; set; }
    
    public static AssignmentResponse ConvertToResponse(Assignment assignment)
    {
        var projectPerformers = assignment.Performers
            .Select(ap => ap.ProjectPerformer).ToList();

        var employee = projectPerformers.Select(pp => pp.Employee).ToList();

        return new AssignmentResponse
        {
            Id = assignment.Id,
            Theme = assignment.Theme,
            Description = assignment.Description ?? string.Empty,
            CreatedAt = assignment.CreatedAt,
            Deadline = assignment.Deadline,
            Budget = assignment.Budget,
            Performers = employee.Select(EmployeeResponse.ConvertToResponse).ToList(),
        };
    }
}