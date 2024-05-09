namespace Server.Payload.DTOs;

public class EmployeeDto
{
    public long? Id { get; set; }
    public string? Email { get; set; }
    public string? Position { get; set; }
    public double? Salary { get; set; }
}