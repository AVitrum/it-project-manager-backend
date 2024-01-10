using System.ComponentModel.DataAnnotations;
using api.Data.Models;

namespace api.Data.Requests;

public class UserCreationRequest
{
    [Required(ErrorMessage = "Username is required.")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "The password must be more than 8 characters long")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", 
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    public required string Password { get; set; }
    
    public static User UserCreationRequestToUser(UserCreationRequest userCreationRequest)
    {
        return new User
        {
            Username = userCreationRequest.Username,
            Email = userCreationRequest.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreationRequest.Password)
        };
    } 
}
