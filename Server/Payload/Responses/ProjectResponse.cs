using DatabaseService.Data.Models;

namespace Server.Payload.Responses;

public class ProjectResponse
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Image { get; set; }
    public required double Budget { get; set; }
    public required List<EmployeeResponse> Performers { get; set; }
    
    public static ProjectResponse ConvertToResponse(Project project)
    {
        var performers = project.ProjectPerformers!.Select(performer => performer.Employee).ToList();

        return new ProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Budget = project.Budget,
            Performers = performers.Select(EmployeeResponse.ConvertToResponse)
                .ToList(),
            Image = project.PictureLink
        };
    }
}