using System;
using Backend.DTOs;
using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> Register(RegisterDto request);
    Task<AuthResponseDto> Login(LoginDto request);
    Task<IdentityUser?> GetUserByEmail(string email);
    Task<bool> Logout(string userId);
    Task<bool> ValidateToken(string userId, string token); 

}
