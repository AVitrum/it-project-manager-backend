using System.ComponentModel.DataAnnotations;

namespace Server.Payload.Requests;

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "The password must be more than 8 characters long")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", 
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    public required string NewPassword { get; set; }
    
    [Compare("SecondNewPassword", ErrorMessage = "Passwords are not the same.")]
    public required string SecondNewPassword { get; set; }
}