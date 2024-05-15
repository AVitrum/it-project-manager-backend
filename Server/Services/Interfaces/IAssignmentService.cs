using DatabaseService.Data.DTOs;
using DatabaseService.Data.Models;
using Server.Payload.Responses;

namespace Server.Services.Interfaces;

public interface IAssignmentService
{
    Task CreateAssignment(long projectId, AssignmentDto assignmentDto);
    Task UpdateAssignment(long id, AssignmentDto assignmentDto);
    Task<AssignmentResponse> GetAssignmentAsync(long id);
    Task<List<AssignmentResponse>> GetAllAssignmentsAsync(long projectId);
}