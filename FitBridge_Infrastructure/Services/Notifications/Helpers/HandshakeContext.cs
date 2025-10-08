using FitBridge_Application.Dtos.Notifications;

namespace FitBridge_Infrastructure.Services.Notifications.Helpers;

public class HandshakeContext
{
    public Func<NotificationDto, NotificationMessage, string, Task> Callback { get; set; }

    public NotificationDto NotificationDto { get; set; }

    public NotificationMessage NotificationMessage { get; set; }
}