using FitBridge_Application.Dtos.Notifications;

namespace FitBridge_Application.Interfaces.Services.Notifications
{
    public interface INotificationsStorageService
    {
        /// <summary>
        /// Saves a notification for a specific user, appending it to the end of the list
        /// </summary>
        /// <param name="userId">The ID of the user to save the notification for</param>
        /// <param name="notificationDto">The notification data to save</param>
        /// <returns>True if the notification was saved successfully, false otherwise</returns>
        Task SaveNotificationAsync(string userId, NotificationDto notificationDto);

        /// <summary>
        /// Retrieves all notifications for a specific user
        /// </summary>
        /// <param name="userId">The ID of the user to get notifications for</param>
        /// <returns>A list of notifications for the specified user</returns>
        Task<List<NotificationDto>> GetNotificationsAsync(string userId);

        /// <summary>
        /// Removes all notifications for a specific user
        /// </summary>
        /// <param name="userId">The ID of the user to clear notifications for</param>
        Task ClearAllNotificationsAsync(string userId);
    }
}