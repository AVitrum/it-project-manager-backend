using System.ComponentModel.DataAnnotations;
using api.Data.Models;

namespace api.Data.DTOs;

public class UserCreationDto
{
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "The password must be more than 8 characters long")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", 
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    public string Password { get; set; } = string.Empty;
    
    public static User UserCreationDtoToUser(UserCreationDto userCreationDto)
    {
        return new User
        {
            Username = userCreationDto.Username,
            Email = userCreationDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreationDto.Password)
        };
    } 
}
