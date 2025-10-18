using System;

namespace Backend.DTOs;

public class AuthResponseDto
{
    public string Email { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty; 

}
