using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DatabaseService.Data.Enums;

namespace DatabaseService.Data.Models;

public class PositionInCompany
{
    [Key] 
    public long Id { get; init; }
    public required long CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public ICollection<UserCompany> UserCompanies { get; init; }
    
    public required string Name { get; set; }
    
    [Column(TypeName = "bigint")]
    public PositionPermissions Permissions { get; set; }
}