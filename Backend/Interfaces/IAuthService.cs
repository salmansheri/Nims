using System;
using Backend.DTOs;
using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.Interfaces;

public interface IAuthService
{
    Task<string> Register(RegisterDto request);
    Task<string> Login(LoginDto request);
    Task<IdentityUser?> GetUserByEmail(string email);

}
