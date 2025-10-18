using System;
using Backend.Data;
using Backend.DTOs;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Text;




namespace Backend.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<AuthService> _logger;
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager; 
    private readonly IRedisService _redisService; 


    public AuthService(UserManager<IdentityUser> userManager, ILogger<AuthService> logger, IConfiguration configuration, RoleManager<IdentityRole> roleManager, IRedisService redisService)
    {
        _userManager = userManager;
        _logger = logger;
        _configuration = configuration;
        _roleManager = roleManager;
        _redisService = redisService; 
    }
    public async Task<IdentityUser?> GetUserByEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            _logger.LogWarning($"No user found with email: {email}");

            return null;



        }

        _logger.LogInformation($"User found with email: {email}");

        return user;


    }

    public async Task<AuthResponseDto> Login(LoginDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new Exception("Invalid Credentials");
        }

        var token = GenerateJwtToken(user);

        await _redisService.SetTokenAsync(user.Id, token, TimeSpan.FromDays(7));

        return new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email!,
            Token = token
        }; 


    }

    public async Task<AuthResponseDto> Register(RegisterDto request)
    {
        var existingUser = _userManager.FindByEmailAsync(request.Email);

        if (existingUser.Result != null)
        {
            throw new Exception("User Already Exists");
        }

        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email

        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        var roleName = "User";

        var roleExists = await _roleManager.RoleExistsAsync(roleName);

        if (!roleExists)
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }

        // Add user to default role 
        await _userManager.AddToRoleAsync(user, "User");

        var token = GenerateJwtToken(user);

        await _redisService.SetTokenAsync(user.Id, token, TimeSpan.FromDays(7));

        return new AuthResponseDto
        {
              UserId = user.Id,
            Email = user.Email!,
            Token = token
            
        };
    }

    public async Task<bool> Logout(string userId)
    {
        return await _redisService.RemoveTokenAsync(userId);
    }

    public async Task<bool> ValidateToken(string userId, string token)
    {
        return await _redisService.isTokenValidAsync(userId, token); 
    }

    private string GenerateJwtToken(IdentityUser user)
    {
        if (user == null)
        {

            throw new ArgumentNullException(nameof(user), "user Cannot be null");
            
        }
        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtKey = _configuration["jwt:Key"];
        string token = string.Empty;

        if (!string.IsNullOrEmpty(jwtKey))
        {
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!)


            };

            var roles = _userManager.GetRolesAsync(user).Result;

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _configuration["jwt:Issuer"],
                Audience = _configuration["jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };

            var createToken = tokenHandler.CreateToken(tokenDescriptor);

            token = tokenHandler.WriteToken(createToken);




        }

        return token; 



    }
}
