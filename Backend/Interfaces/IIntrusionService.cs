using System;
using Backend.Models;

namespace Backend.Interfaces;

public interface IIntrusionService
{
    Task<List<IntrusionDetection?>> GetAllInstrusionsAsync(int page = 1, int pageSize = 10);
    Task<IntrusionDetection> GetIntrusionByIdAsync(int id);
    Task<IntrusionDetection> CreateIntrusionAsync(IntrusionDetection intrusion);
    Task ResolveIntrusionAsync(int id, string resolvedByUserId);
    Task<List<IntrusionDetection>> GetRecentIntrusionsAsync(int count = 10);
    Task<DashboardStats> GetDashboardStatsAsync();

}

    public class DashboardStats
    {
        public int TotalIntrusions { get; set; }
        public int ResolvedIntrusions { get; set; }
        public int HighSeverityIntrusions { get; set; }
        public int TodayIntrusions { get; set; }
    }
