using DatabaseService.Data.DTOs;
using DatabaseService.Data.Enums;
using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using FileService;
using Server.Payload.Responses;
using Server.Services.Interfaces;

namespace Server.Services.Implementations;

public class AssignmentService(
    IAssignmentRepository assignmentRepository,
    IProjectRepository projectRepository,
    ICompanyRepository companyRepository,
    IUserRepository userRepository,
    IFileService fileService)
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
            CreatedAt = DateTime.UtcNow,
            Deadline = utcDateTime.AddHours(-3),
            UpdateAt = DateTime.UtcNow
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
        
        var newChange = new AssignmentHistory
        {
            AssignmentId = assignment.Id,
            UpdatedAt = DateTime.UtcNow,
            Change = $"{employee.User!.Username} have updated"
        };
        
        var dateTime = DateTime.Parse(assignmentDto.Deadline);
        var utcDateTime = dateTime.ToUniversalTime();
        
        if (assignment.Deadline < DateTime.UtcNow.AddHours(3) && assignment.Deadline.Equals(utcDateTime.AddHours(-3)))
        {
            assignment.Type = AssignmentType.OVERDUE;
            await assignmentRepository.UpdateAsync(assignment);
            throw new AssignmentDeadlineException();
        }
        
        if (!assignment.Deadline.Equals(utcDateTime.AddHours(-3)))
        {
            assignment.Deadline = utcDateTime.AddHours(-3);
            newChange.Change += " deadline;";
        }

        if (assignment.Theme != assignmentDto.Theme && assignmentDto.Theme != null)
        {
            assignment.Theme = assignmentDto.Theme;
            newChange.Change += " theme;";
        }

        if (assignment.Description != assignmentDto.Description && assignmentDto.Description != null)
        {
            assignment.Description = assignmentDto.Description;
            newChange.Change += " description;";
        }
        
        if (!((int)assignment.Budget).Equals((int)assignmentDto.Budget) && assignmentDto.Budget != 0)
        {
            assignment.Budget = assignmentDto.Budget;
            newChange.Change += " budget;";
        }

        await assignmentRepository.AddChange(newChange);
        
        assignment.UpdateAt = DateTime.UtcNow;
        await assignmentRepository.UpdateAsync(assignment);
    }

    public async Task AddFile(long id, IFormFile file)
    {
        var assignment = await assignmentRepository.GetByIdAsync(id);

        if (assignment.Type == AssignmentType.OVERDUE)
        {
            throw new AssignmentDeadlineException();
        }

        var company = await companyRepository.GetByIdAsync(assignment.Project!.CompanyId);
        var employee = 
            await companyRepository.GetEmployeeByUserAndCompanyAsync(await userRepository.GetByJwtAsync(), company);
        var projectPerformer = 
            await projectRepository.GetPerformerByEmployeeAndProjectAsync(employee, assignment.Project);
        await assignmentRepository.GetPerformerByProjectPerformerAndAssignment(projectPerformer, assignment);

        var folder = $"{company.Name}/{assignment.Project.Name}/{assignment.Theme}";
        var (link, dbFileName) = await fileService.UploadFileAsync(folder, file);

        var newFile = new AssignmentFile
        {
            AssignmentId = assignment.Id,
            Name = file.FileName,
            DbName = dbFileName,
            Link = link
        };
        await assignmentRepository.AddFile(newFile);
    }

    public async Task AddComment(long id, CommentDto commentDto)
    {
        var assignment = await assignmentRepository.GetByIdAsync(id);

        var company = await companyRepository.GetByIdAsync(assignment.Project!.CompanyId);
        var employee = 
            await companyRepository.GetEmployeeByUserAndCompanyAsync(await userRepository.GetByJwtAsync(), company);
        var projectPerformer = 
            await projectRepository.GetPerformerByEmployeeAndProjectAsync(employee, assignment.Project);
        var performer =
            await assignmentRepository.GetPerformerByProjectPerformerAndAssignment(projectPerformer, assignment);
        
        var comment = new Comment
        {
            UserId = performer.Id,
            Message = commentDto.Message,
            CreatedAt = DateTime.UtcNow,
            AssignmentId = id
        };

        await assignmentRepository.AddComment(comment);
    }

    public async Task<AssignmentResponse> GetAssignmentAsync(long id)
    {
        var assignment = await assignmentRepository.GetByIdAsync(id);
        assignment.Changes = await assignmentRepository.GetChanges(assignment);
        assignment.Files = await assignmentRepository.GetAllFiles(assignment);

        return AssignmentResponse.ConvertToResponse(assignment);
    }

    public async Task<List<AssignmentResponse>> GetAllAssignmentsAsync(long projectId)
    {
        var assignments = await assignmentRepository.GetAllByProjectIdAsync(projectId);

        foreach (var assignment in assignments)
        {
            assignment.Changes = await assignmentRepository.GetChanges(assignment);
            assignment.Files = await assignmentRepository.GetAllFiles(assignment);
            
            if (assignment.Deadline >= DateTime.UtcNow.AddHours(3) 
                || assignment.Type == AssignmentType.COMPLETED) continue;
            assignment.Type = AssignmentType.OVERDUE;
            await assignmentRepository.UpdateAsync(assignment);
        }

        return assignments.Select(AssignmentResponse.ConvertToResponse).ToList();
    }
}