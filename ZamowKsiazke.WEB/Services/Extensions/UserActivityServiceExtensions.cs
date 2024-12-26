using System.Threading.Tasks;
using ZamowKsiazke.Services.Interfaces;

namespace ZamowKsiazke.Services.Extensions
{
    public static class UserActivityServiceExtensions
    {
        public static async Task LogLoginAsync(this IUserActivityService activityService, string userId)
        {
            await activityService.LogActivityAsync(userId, "Login", "Użytkownik zalogował się do systemu");
        }

        public static async Task LogLogoutAsync(this IUserActivityService activityService, string userId)
        {
            await activityService.LogActivityAsync(userId, "Logout", "Użytkownik wylogował się z systemu");
        }

        public static async Task LogOrderCreatedAsync(this IUserActivityService activityService, string userId, string orderId)
        {
            await activityService.LogActivityAsync(userId, "Order", "Złożono nowe zamówienie", orderId);
        }

        public static async Task LogProfileUpdateAsync(this IUserActivityService activityService, string userId)
        {
            await activityService.LogActivityAsync(userId, "ProfileUpdate", "Zaktualizowano profil użytkownika");
        }

        public static async Task LogBookActionAsync(this IUserActivityService activityService, string userId, string action, string bookId)
        {
            await activityService.LogActivityAsync(userId, $"Book{action}", $"Wykonano akcję {action} na książce", bookId);
        }
    }
}
