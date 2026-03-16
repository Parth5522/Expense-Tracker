using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface INotificationService
{
    Task<List<Notification>> GetNotificationsAsync(string userId);
    Task<int> GetUnreadCountAsync(string userId);
    Task MarkAsReadAsync(int id);
    Task MarkAllAsReadAsync(string userId);
    Task CreateNotificationAsync(string userId, NotificationType type, string message);
}
