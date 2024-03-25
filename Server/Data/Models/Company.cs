using System.ComponentModel.DataAnnotations;

namespace Server.Data.Models;

public class Company
{
    [Key]
    public long Id { get; set; }

    public string Name { get; set; }
    
    public ICollection<UserCompany> UserCompanies { get; set; }
}

