using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Services.Notifications.UserNotifications;
using FitBridge_Infrastructure.Services.Notifications.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace FitBridge_Infrastructure.Services.Notifications
{
    internal class NotificationsBackgroundService(
        ChannelReader<NotificationMessage> channelReader,
        IHubContext<NotificationHub, IUserNotifications> hubContext,
        ILogger<NotificationsService> logger,
        NotificationStorageService notificationsStorageService) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested && await channelReader.WaitToReadAsync(stoppingToken))
            {
                while (channelReader.TryRead(out var notificationMessage))
                {
                    foreach (Guid userId in notificationMessage.UserIds)
                    {
                        //
                    }
                }
            }
        }

        //private NotificationDto BuildNotificationDto(NotificationMessage notificationMessage)
        //{
        //}
    }
}