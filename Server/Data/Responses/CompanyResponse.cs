namespace Server.Data.Responses;

public class CompanyResponse
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public required List<UserCompanyResponse> Users { get; set; }
}