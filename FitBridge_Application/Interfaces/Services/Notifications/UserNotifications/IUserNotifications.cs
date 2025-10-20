using FitBridge_Application.Dtos.Notifications;

namespace FitBridge_Application.Interfaces.Services.Notifications.UserNotifications
{
    public interface IUserNotifications
    {
        public Task NotificationReceived(NotificationDto notificationDto);
    }
}