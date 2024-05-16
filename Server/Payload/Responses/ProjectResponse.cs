using DatabaseService.Data.Models;

namespace Server.Payload.Responses;

public class ProjectResponse
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Image { get; set; }
    public required double Budget { get; set; }
    public required long CompanyId { get; set; }
    public required List<AssignmentResponse> Assignments { get; set; } = [];
    public required List<EmployeeResponse> Performers { get; set; } = [];
    
    
    public static ProjectResponse ConvertToResponse(Project project)
    {
        var employees = project.ProjectPerformers!.Select(performer => performer.Employee).ToList();
        var performers = employees.Select(EmployeeResponse.ConvertToResponse).ToList();

        return new ProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Budget = project.Budget,
            Performers = performers,
            Image = project.PictureLink,
            Assignments = project.Assignments.Select(AssignmentResponse.ConvertToResponse)
                .ToList(),
            CompanyId = project.CompanyId,
        };
    }
}