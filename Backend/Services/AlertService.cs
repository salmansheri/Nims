using System;
using Backend.Data;
using Backend.Hubs;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class AlertService: IAlertService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<AlertHub> _hubContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<AlertService> _logger;

    public AlertService(ApplicationDbContext context, IHubContext<AlertHub> hubContext, UserManager<IdentityUser> userManager, ILogger<AlertService> logger)
    {
        _context = context;
        _hubContext = hubContext;
        _userManager = userManager;
        _logger = logger; 
    }

    public async Task AcknowledgeAlertAsync(int id, string acknowledgedByUserId)
    {
        var alert = await _context.Alerts.FindAsync(id); 
        if (alert != null)
        {
            alert.IsAcknowledged = true;
                alert.AcknowledgedById = acknowledgedByUserId;
                alert.AcknowledgedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Alert with ID {id} acknowledged by user {acknowledgedByUserId}");
        }
       
    }

    public async Task<Alert> CreateAlertAsync(Alert alert)
    {
        var newAlert = _context.Alerts.Add(alert);

        if (newAlert == null)
        {
            _logger.LogError("Failed to create alert");
            throw new Exception("Failed to create alert");
        }

        await _context.SaveChangesAsync();
        return newAlert.Entity; 
    }

    public async Task<List<Alert>> GetAlertsAsync(bool unAcknowledgedOnly = false)
    {
        var query = _context.Alerts
            .Include(a => a.AcknowledgedBy)
            .AsQueryable();

        if (unAcknowledgedOnly)
        {
            query = query.Where(a => a.IsAcknowledged);
        }

        var alerts = await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        if (alerts == null || alerts.Count == 0)
        {
            _logger.LogInformation("No alerts found");
            return null!;
        }

        return alerts;
    }

    public async Task<int> GetUnacknowledgedCountAsync()
    {
        return await _context.Alerts.CountAsync(a => !a.IsAcknowledged); 
    }
    
    
}
