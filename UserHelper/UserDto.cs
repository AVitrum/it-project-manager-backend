using System.ComponentModel.DataAnnotations;

namespace UserHelper;

public class UserDto
{
    public string? Username { get; set; } 
    public string? Email { get; set; } 
    [RegularExpression(@"^\+\d{1,3}\d{9,}$", ErrorMessage = "Phone number must be in the format +XXXXXXXXXXXX")]
    public string? Phone { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public DateTime? RegistrationDate { get; set; }
}