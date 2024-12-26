using System;
using System.Collections.Generic;
using ZamowKsiazke.Models;

namespace ZamowKsiazke.ViewModel
{
    public class UserActivityReportViewModel
    {
        public UserActivityReportViewModel()
        {
            RecentActivities = new List<UserActivity>();
            ActivityTypeStats = new Dictionary<string, int>();
            UserLoginStats = new Dictionary<string, int>();
            ReportGeneratedAt = DateTime.UtcNow;
        }

        public List<UserActivity> RecentActivities { get; set; }
        public Dictionary<string, int> ActivityTypeStats { get; set; }
        public Dictionary<string, int> UserLoginStats { get; set; }
        public int TotalActivities { get; set; }
        public int UniqueUsers { get; set; }
        public DateTime ReportGeneratedAt { get; set; }
    }

    public class UserActivitySummary
    {
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public int TotalActivities { get; set; }
        public DateTime LastActive { get; set; }
        public int OrdersCount { get; set; }
        public int LoginCount { get; set; }
    }
}
