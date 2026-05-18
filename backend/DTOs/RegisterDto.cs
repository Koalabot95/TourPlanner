using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class RegisterDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)]           // Between 3 and 50 Characters
    [RegularExpression(@"^[a-zA-Z0-9_]+$")]         // Only letters, numbers and underscores allowed
    public string Username { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).{8,}$")]       //Min: 8 characters, 1 uppercase, 1 lowercase, 1 digit, 1 special character
    public string Password { get; set; } = null!;                                       
}