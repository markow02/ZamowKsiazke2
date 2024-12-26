using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZamowKsiazke.Data;
using ZamowKsiazke.Models;
using ZamowKsiazke.Services.Interfaces;

namespace ZamowKsiazke.Services
{
    public class UserActivityService : IUserActivityService
    {
        private readonly ZamowKsiazkeContext _context;

        public UserActivityService(ZamowKsiazkeContext context)
        {
            _context = context;
        }

        public async Task LogActivityAsync(string userId, string activityType, string description, string? relatedEntityId = null)
        {
            var activity = new UserActivity
            {
                UserId = userId,
                ActivityType = activityType,
                Description = description,
                RelatedEntityId = relatedEntityId,
                Timestamp = DateTime.UtcNow
            };

            await _context.UserActivities.AddAsync(activity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserActivity>> GetUserActivitiesAsync(string userId)
        {
            return await _context.UserActivities
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Include(a => a.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserActivity>> GetAllActivitiesAsync()
        {
            return await _context.UserActivities
                .OrderByDescending(a => a.Timestamp)
                .Include(a => a.User)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetActivityStatisticsAsync()
        {
            var activities = await _context.UserActivities
                .GroupBy(a => a.ActivityType)
                .Select(g => new { ActivityType = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ActivityType, x => x.Count);

            return activities;
        }

        public async Task<List<UserActivity>> GetRecentActivitiesAsync(int count = 50)
        {
            return await _context.UserActivities
                .OrderByDescending(a => a.Timestamp)
                .Include(a => a.User)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetUserLoginStatisticsAsync(int days = 30)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            var loginStats = await _context.UserActivities
                .Where(a => a.ActivityType == "Login" && a.Timestamp >= startDate)
                .GroupBy(a => a.UserId)
                .Select(g => new { UserId = g.Key, LoginCount = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.LoginCount);

            return loginStats;
        }
    }
}
