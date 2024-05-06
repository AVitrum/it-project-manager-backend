namespace DatabaseService.Data.DTOs;

public class AssignmentDto
{
    public string? Theme { get; set; }
    public string? Description { get; set; }
    public double Budget { get; set; } = 0;
}