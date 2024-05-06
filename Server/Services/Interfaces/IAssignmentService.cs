using DatabaseService.Data.DTOs;

namespace Server.Services.Interfaces;

public interface IAssignmentService
{
    Task CreateAssignment(long projectId, AssignmentDto assignmentDto);
}