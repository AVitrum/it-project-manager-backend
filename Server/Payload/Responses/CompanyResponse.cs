namespace Server.Payload.Responses;

public class CompanyResponse
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required double Budget { get; set; }
    public required double AverageSalary { get; set; }
    public required double MaxSalary { get; set; }
    public required double MinSalary { get; set; }
    public required double TotalCosts { get; set; }
    public string? Picture { get; set; } 
    public required List<EmployeeResponse> Employees { get; set; }
    public required List<ProjectResponse> Projects { get; set; }
}