using DatabaseService.Data.DTOs;
using DatabaseService.Data.Enums;
using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using FileService;
using Server.Payload.Responses;
using Server.Services.Interfaces;

namespace Server.Services.Implementations;

public class ProjectService(
    IProjectRepository projectRepository,
    ICompanyRepository companyRepository, 
    IUserRepository userRepository,
    IFileService fileService) 
    : IProjectService
{
    public async Task CreateAsync(long companyId, ProjectDto projectDto)
    {
        var user = await userRepository.GetByJwtAsync();
        var company = await companyRepository.GetByIdAsync(companyId);
        var performer = await companyRepository.GetEmployeeByUserAndCompanyAsync(user, company);

        if (!performer.PositionInCompany.HasPermissions(PositionPermissions.CreateProject))
        {
            throw new PermissionException();
        }

        if (company.Budget - projectDto.Budget < 0)
        {
            throw new ProjectException("You don't have enough money");
        }

        var newProject = new Project
        {
            Budget = projectDto.Budget,
            Description = projectDto.Description ?? string.Empty,
            CreatorId = performer.Id,
            CompanyId = company.Id,
            Name = projectDto.Name!,
            CreationDate = DateTime.UtcNow,
        };
        var project = await projectRepository.CreateAsync(newProject);

        var projectPerformer = new ProjectPerformer
        {
            EmployeeId = performer.Id,
            ProjectId = project.Id,
        };
        await projectRepository.AddPerformer(projectPerformer);
    }

    public async Task UpdateAsync(long projectId, ProjectDto projectDto)
    {
        var user = await userRepository.GetByJwtAsync();
        var project = await projectRepository.GetByIdAsync(projectId);
        var performer = await companyRepository.GetEmployeeByUserAndCompanyAsync(user, project.Company!);

        if (!performer.PositionInCompany.HasPermissions(PositionPermissions.UpdateProject))
        {
            throw new PermissionException();
        }

        if (projectDto.Name != null)
        {
            project.Name = projectDto.Name;
        }

        if (projectDto.Description != null)
        {
            project.Description = projectDto.Description;
        }

        if (projectDto.Budget != 0)
        {
            if (!performer.PositionInCompany.HasPermissions(PositionPermissions.UpdateBudget))
            {
                throw new PermissionException();
            }
            
            if (project.Company!.Budget - projectDto.Budget < 0)
            {
                throw new ProjectException("You don't have enough money");
            }

            project.Budget = projectDto.Budget;
        }

        await projectRepository.UpdateAsync(project);
    }

    public async Task ChangeProjectImage(long projectId, IFormFile file)
    {
        var project = await projectRepository.GetByIdAsync(projectId);
        
        fileService.CheckImage(file);
        
        if (project.PictureName != null)
            await fileService.DeleteAsync(project.PictureName);
        
        var imageUrl = await fileService.UploadAsync(project.Name, file);

        project.PictureLink = imageUrl;
        project.PictureName = $"{project.Name}_{file.FileName}";
        await projectRepository.UpdateAsync(project);
    }
    
    public async Task<ProjectResponse> GetProjectInfoAsync(long projectId)
    {
        var project = await projectRepository.GetByIdAsync(projectId);
        var res = ProjectResponse.ConvertToResponse(project);
        return res;
    }

    public async Task AddPerformerAsync(long projectId, PerformerDto performerDto)
    {
        var project = await projectRepository.GetByIdAsync(projectId);

        if (await projectRepository.PerformerExistsByEmail(performerDto.Email, projectId))
        {
            throw new ProjectException("Performer Already In Project");
        }
        
        var employeeToAdd = await companyRepository.GetEmployeeByUserAndCompanyAsync(
            await userRepository.GetByEmailAsync(performerDto.Email),
            await companyRepository.GetByIdAsync(project.CompanyId));

        await projectRepository.AddPerformer(new ProjectPerformer
        {
            EmployeeId = employeeToAdd.Id,
            ProjectId = projectId,
            Employee = employeeToAdd,
            Project = project
        });
    }

    public async Task<List<ProjectResponse>> GetAllProjectsAsync(long companyId)
    {
        var projects = await projectRepository.GetAllByCompanyIdAsync(companyId);

        var response = projects.Select(ProjectResponse.ConvertToResponse).ToList();

        return response;
    }
}