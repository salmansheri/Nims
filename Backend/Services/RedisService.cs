using System;
using Backend.Interfaces;
using Microsoft.AspNetCore.Connections;
using StackExchange.Redis;

namespace Backend.Services;

public class RedisService : IRedisService
{

    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public RedisService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _database = redis.GetDatabase(); 
        
    }
    public async Task<string?> GetTokenAsync(string userId)
    {
        var key = $"auth_token:{userId}";
        return await _database.StringGetAsync(key); 
    }

    public async Task<bool> isTokenValidAsync(string userId, string token)
    {
        var storedToken = await GetTokenAsync(userId);
        return storedToken == token; 
    }

    public async Task<bool> KeyExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key); 
    }

    public async Task<bool> RemoveTokenAsync(string userId)
    {
        var key = $"auth_token:{userId}";
        return await _database.KeyDeleteAsync(key); 
    }

    public async Task<bool> SetTokenAsync(string userId, string token, TimeSpan expiry)
    {
        var key = $"auth_token:{userId}";
        return await _database.StringSetAsync(key, token, expiry); 
    }
}
