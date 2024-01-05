using System.ComponentModel.DataAnnotations;

namespace api.Data.Models;

public class User
{

    [Key]
    public long Id { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Username { get; set; }
    
    [Required]
    [MaxLength(40)]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }
    
    
    public string PasswordHash { get; set; }
}