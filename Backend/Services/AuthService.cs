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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web; 




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
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
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

    


    private async Task<IdentityUser?> ValidateUserAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtkey = _configuration["jwt:Key"];

            if (string.IsNullOrEmpty(jwtkey))
                throw new InvalidOperationException("Jwt key is not configured");

            var key = Encoding.ASCII.GetBytes(jwtkey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero

            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken)
                return null;

            if (jwtToken.Header.Alg != SecurityAlgorithms.HmacSha256)
                return null;

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return null;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            return user;


        }
        catch (SecurityTokenInvalidSignatureException)
        {
            Console.WriteLine("JWT token signature is invalid");
            return null;
        }
        catch (SecurityTokenException ex)
        {
            Console.WriteLine($"JWT token validation failed: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error during token validation: {ex.Message}");
            return null;
        }
    }

    public async Task<(IdentityUser User, IList<string> Roles)?> GetCurrentUserAsync(string token)
    {
        var user = await ValidateUserAsync(token);

        if (user == null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        return (user, roles);

    } 
    
    public async Task<string>  GetUserIdAsync(string userEmail)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user == null)
        {
            return ""; 
        }
        return user.Id;
        
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
