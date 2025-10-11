using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public required string Password { get; set; }
}