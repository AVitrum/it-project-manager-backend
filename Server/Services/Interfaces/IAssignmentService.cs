using DatabaseService.Data.DTOs;
using DatabaseService.Data.Models;
using Server.Payload.Responses;

namespace Server.Services.Interfaces;

public interface IAssignmentService
{
    Task CreateAssignment(long projectId, AssignmentDto assignmentDto);
    Task AddPerformer(long assignmentId, PerformerDto performerDto);
    Task ImportAllTasksToCalendar(long projectId);
    Task ToReview(long id);
    Task MarkAsCompleted(long id);
    Task ReturnTask(long id);
    Task UpdateAssignment(long id, AssignmentDto assignmentDto);
    Task AddFile(long id, IFormFile file);
    Task AddComment(long id, CommentDto commentDto);
    Task<AssignmentResponse> GetAssignmentAsync(long id);
    Task<List<AssignmentResponse>> GetAllAssignmentsAsync(long projectId);
}