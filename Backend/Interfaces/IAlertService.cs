using System;
using Backend.Models;

namespace Backend.Interfaces;

public interface IAlertService
{
    Task<Alert> CreateAlertAsync(Alert alert);
    Task<List<Alert>> GetAlertsAsync(bool unAcknowledgedOnly = false);
    Task AcknowledgeAlertAsync(int id, string acknowledgedByUserId);
    Task<int> GetUnacknowledgedCountAsync();  

}
