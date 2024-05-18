using DatabaseService.Data.Models;

namespace Server.Payload.Responses;

public class AssignmentResponse
{
    public required long Id { get; set; }
    public required long CompanyId { get; set; }
    public required long ProjectId { get; set; }
    public required string Theme { get; set; }
    public required string Description { get; set; }
    public required string Type { get; set; }
    public required double Budget { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
    public required DateTime Deadline { get; set; }
    public required List<EmployeeResponse> Performers { get; set; }
    public required List<HistoryResponse> Histories { get; set; }
    public required List<CommentResponse> Comments { get; set; }
    public required List<FileResponse> Files { get; set; }


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
            UpdatedAt = assignment.UpdateAt,
            Budget = assignment.Budget,
            Performers = employee.Select(EmployeeResponse.ConvertToResponse)
                .ToList(),
            Comments = assignment.Comments.Select(CommentResponse.ConvertToResponse)
                .ToList(),
            Histories = assignment.Changes.Select(HistoryResponse.ConvertToResponse)
                .ToList(),
            Type = assignment.Type.ToString(),
            Files = assignment.Files.Select(FileResponse.ConvertToResponse)
                .ToList(),
            CompanyId = assignment.Project!.CompanyId,
            ProjectId = assignment.ProjectId,
        };
    }
}