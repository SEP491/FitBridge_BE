using FitBridge_Application.Dtos.Notifications;

namespace FitBridge_Application.Interfaces.Services.Notifications
{
    public interface INotificationService
    {
        public Task NotifyUsers(NotificationMessage notificationMessage);
    }
}