using System;

namespace Backend.Interfaces;

public interface IRedisService
{
    Task<bool> SetTokenAsync(string userId, string token, TimeSpan expiry);
    Task<string?> GetTokenAsync(string userId);
    Task<bool> RemoveTokenAsync(string userId);
    Task<bool> isTokenValidAsync(string userId, string token);
    Task<bool> KeyExistsAsync(string key); 

}
