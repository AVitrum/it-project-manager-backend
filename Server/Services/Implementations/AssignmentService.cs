using DatabaseService.Data.DTOs;
using DatabaseService.Data.Enums;
using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using Server.Services.Interfaces;

namespace Server.Services.Implementations;

public class AssignmentService(
    IAssignmentRepository assignmentRepository,
    IProjectRepository projectRepository,
    ICompanyRepository companyRepository,
    IUserRepository userRepository)
    : IAssignmentService
{
    public async Task CreateAssignment(long projectId, AssignmentDto assignmentDto)
    {
        var project = await projectRepository.GetByIdAsync(projectId);

        var performer = await projectRepository.GetPerformerByEmployeeAndProjectAsync(
            await companyRepository.GetEmployeeByUserAndCompanyAsync(
                await userRepository.GetByJwtAsync(), project.Company),
            project);

        if (!performer.Employee.PositionInCompany.HasPermissions(PositionPermissions.CreateTask))
        {
            throw new PermissionException();
        }
            
        var newAssignment = new Assignment
        {
            ProjectId = projectId,
            Project = project,
            Theme = assignmentDto.Theme!,
            CreatedAt = DateTime.UtcNow,
            Deadline = DateTime.UtcNow
        };

        await assignmentRepository.CreateAsync(newAssignment);

        var assignment = await assignmentRepository.GetByThemeAsync(newAssignment.Theme);

        var assignmentPerformer = new AssignmentPerformer
        {
            ProjectPerformerId = projectId,
            ProjectPerformer = performer,
            AssignmentId = assignment.Id,
            Assignment = assignment
        };
        await assignmentRepository.AddPerformer(assignmentPerformer);
    }
}