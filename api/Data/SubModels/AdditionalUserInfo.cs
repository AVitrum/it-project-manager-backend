using System.ComponentModel.DataAnnotations;
using api.Data.Models;

namespace api.Data.SubModels;

public class AdditionalUserInfo
{
    [Key, Required]
    public long Id { get; set; }
    
    [Required]
    public required string Type { get; set; }
    
    [Required]
    public required string Info { get; set; }
    
    public long UserId { get; set; }
    public User User { get; set; }
}