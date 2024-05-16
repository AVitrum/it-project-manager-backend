namespace DatabaseService.Data.DTOs;

public class ProjectDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double Budget { get; set; } = 0;
}