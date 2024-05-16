using DatabaseService.Data.DTOs;
using DatabaseService.Data.Enums;
using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using Server.Payload.Responses;
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

        if (project.Budget < assignmentDto.Budget)
        {
            throw new ProjectException("You don't have enough money");
        }

        var company = await companyRepository.GetByIdForOperations(project.CompanyId);
        
        var employee = await companyRepository.GetEmployeeByUserAndCompanyAsync(
            await userRepository.GetByJwtAsync(), company);
        
        var performer = await projectRepository.GetPerformerByEmployeeAndProjectAsync(employee, project);

        if (!performer.Employee.PositionInCompany.HasPermissions(PositionPermissions.CreateTask))
        {
            throw new PermissionException();
        }
        var dateTime = DateTime.Parse(assignmentDto.Deadline);
        var utcDateTime = dateTime.ToUniversalTime();
        
        var newAssignment = new Assignment
        {
            ProjectId = projectId,
            Project = project,
            Description = assignmentDto.Description,
            Budget = assignmentDto.Budget,
            Theme = assignmentDto.Theme!,
            CreatedAt = DateTime.UtcNow.AddHours(3),
            Deadline = utcDateTime.AddHours(-3)
        };
        var assignment = await assignmentRepository.CreateAsync(newAssignment);

        var assignmentPerformer = new AssignmentPerformer
        {
            ProjectPerformerId = performer.Id,
            ProjectPerformer = performer,
            AssignmentId = assignment.Id,
            Assignment = assignment
        };
        await assignmentRepository.AddPerformer(assignmentPerformer);
    }

    public async Task UpdateAssignment(long id, AssignmentDto assignmentDto)
    {
        var assignment = await assignmentRepository.GetByIdAsync(id);
        
        var company = await companyRepository.GetByIdForOperations(assignment.Project!.CompanyId);
        
        var employee = await companyRepository.GetEmployeeByUserAndCompanyAsync(
            await userRepository.GetByJwtAsync(), company);
        
        var performer = await projectRepository.GetPerformerByEmployeeAndProjectAsync(employee, assignment.Project);
       
        if (!performer.Employee.PositionInCompany.HasPermissions(PositionPermissions.UpdateTask))
        {
            throw new PermissionException();
        }
        
        var dateTime = DateTime.Parse(assignmentDto.Deadline);
        var utcDateTime = dateTime.ToUniversalTime();

        assignment.Theme = assignmentDto.Theme!;
        assignment.Description = assignmentDto.Description;
        assignment.Budget = assignmentDto.Budget;
        assignment.Deadline = utcDateTime.AddHours(-2);

        await assignmentRepository.UpdateAsync(assignment);
    }

    public async Task<AssignmentResponse> GetAssignmentAsync(long id)
    {
        var assignment = await assignmentRepository.GetByIdAsync(id);

        return AssignmentResponse.ConvertToResponse(assignment);
    }

    public async Task<List<AssignmentResponse>> GetAllAssignmentsAsync(long projectId)
    {
        var assignments = await assignmentRepository.GetAllByProjectIdAsync(projectId);

        return assignments.Select(AssignmentResponse.ConvertToResponse).ToList();
    }
}