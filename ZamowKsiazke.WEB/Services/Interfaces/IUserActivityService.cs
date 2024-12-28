using ZamowKsiazke.Models;

namespace ZamowKsiazke.Services.Interfaces
{
    public interface IUserActivityService
    {
        Task LogActivityAsync(string userId, string activityType, string description, string? relatedEntityId = null);
        Task<IEnumerable<UserActivity>> GetUserActivitiesAsync(string userId);
        Task<IEnumerable<UserActivity>> GetAllActivitiesAsync();
        Task<Dictionary<string, int>> GetActivityStatisticsAsync();
        Task<List<UserActivity>> GetRecentActivitiesAsync(int count = 50);
        Task<Dictionary<string, int>> GetUserLoginStatisticsAsync(int days = 30);
    }
}
