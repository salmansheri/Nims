using System;
using Backend.Data;
using Backend.Hubs;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class IntrusionService : IIntrusionService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<AlertHub> _hubContext;
    private readonly ILogger<IntrusionService> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public IntrusionService(ApplicationDbContext context, IHubContext<AlertHub> hubContext, UserManager<IdentityUser> userManger, ILogger<IntrusionService> logger)
    {
        _context = context;
        _hubContext = hubContext;
        _userManager = userManger;
        _logger = logger; 


    }
    public async Task<IntrusionDetection> CreateIntrusionAsync(IntrusionDetection intrusion)
    {
        _context.IntrusionDetections.Add(intrusion);
        await _context.SaveChangesAsync();

        // Create alert for high severity intrusions
        if (intrusion.Severity >= 3)
        {
            var alert = new Alert
            {
                Title = $"High Severity Intrusion Detected - {intrusion.AttackType}",
                Description = $"Intrusion from {intrusion.SourceIP} to {intrusion.DestinationIP} detected",
                Severity = intrusion.Severity,
                AlertType = "Intrusion"


            };

            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveAlert", alert);


        }

            _logger.LogInformation($"Intrusion detected: {intrusion.AttackType} from {intrusion.SourceIP} to {intrusion.DestinationIP}");

        return intrusion; 
    }

    public async Task<List<IntrusionDetection?>> GetAllInstrusionsAsync(int page = 1, int pageSize = 10)
    {
        var intrusions = await _context.IntrusionDetections
            .Include(i => i.ResolvedBy)
            .OrderByDescending(i => i.DetectedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        if (intrusions == null || intrusions.Count == 0)
        {
            _logger.LogWarning("No intrusions found in the database.");
            return new List<IntrusionDetection?>();
        }

        return intrusions!; 
        }

        
    

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        var totalIntrusions = await _context.IntrusionDetections.CountAsync();
        var resolvedIntrusions = await _context.IntrusionDetections.CountAsync(i => i.IsResolved);
        var highSeverityIntrusions = await _context.IntrusionDetections.CountAsync(i => i.Severity >= 4);
        var todayIntrusions = await _context.IntrusionDetections.CountAsync(i => i.DetectedAt.Date == DateTime.UtcNow.Date);

        return new DashboardStats
        {
             TotalIntrusions = totalIntrusions,
                ResolvedIntrusions = resolvedIntrusions,
                HighSeverityIntrusions = highSeverityIntrusions,
                TodayIntrusions = todayIntrusions


        }; 
    }

    public Task<IntrusionDetection> GetIntrusionByIdAsync(int id)
    {
        var intrusion = _context.IntrusionDetections
            .Include(i => i.ResolvedBy)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (intrusion == null)
        {
            _logger.LogWarning($"Intrusion with ID {id} not found.");
            return null!;
        }

        return intrusion!; 
        
    }

    public async Task<List<IntrusionDetection>> GetRecentIntrusionsAsync(int count = 10)
    {
        var recentIntrusions = await _context.IntrusionDetections
            .Include(i => i.ResolvedBy)
            .OrderByDescending(i => i.DetectedAt)
            .Take(count)
            .ToListAsync();

        if (recentIntrusions == null || recentIntrusions.Count == 0)
        {
            _logger.LogWarning("No recent intrusions found in the database.");
            return null!;
        }

        return recentIntrusions; 
    }

    public async Task ResolveIntrusionAsync(int id, string resolvedByUserId)
    {
        var intrusion = await _context.IntrusionDetections.FindAsync(id); 

        if (intrusion != null)
        {
             intrusion.IsResolved = true;
                intrusion.ResolvedById = resolvedByUserId;
                intrusion.ResolvedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Intrusion with ID {id} resolved by user {resolvedByUserId}.");
            
        }
    }
}
