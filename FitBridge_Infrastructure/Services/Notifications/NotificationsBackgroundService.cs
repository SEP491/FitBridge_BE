using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Services.Notifications.UserNotifications;
using FitBridge_Application.Specifications.Notifications;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Infrastructure.Services.Notifications.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace FitBridge_Infrastructure.Services.Notifications
{
    internal class NotificationsBackgroundService(
        ChannelReader<NotificationMessage> channelReader,
        IHubContext<NotificationHub, IUserNotifications> hubContext,
        ILogger<NotificationsService> logger,
        NotificationStorageService notificationsStorageService,
        NotificationConnectionManager notificationConnectionManager,
        PushNotificationService pushNotificationService,
        NotificationHandshakeManager notificationHandshakeManager,
        IUnitOfWork unitOfWork) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested && await channelReader.WaitToReadAsync(stoppingToken))
            {
                while (channelReader.TryRead(out var notificationMessage))
                {
                    foreach (Guid userId in notificationMessage.UserIds)
                    {
                        var isExists = await notificationConnectionManager.IsConnectionExistsAsync(userId.ToString());
                        var dto = new NotificationDto(notificationMessage.NotificationPayload);
                        if (isExists)
                        {
                            dto.Title = notificationMessage.InAppNotificationTemplate!.TemplateTitle;
                            dto.Body = notificationMessage.InAppNotificationTemplate.TemplateBody;

                            logger.LogInformation("Template title {Title}; Template body {Body}; User id: {Id}", dto.Title, dto.Body, userId.ToString());
                            await hubContext.Clients.User(userId.ToString()).NewNotification(dto);
                            notificationHandshakeManager.StartHandshake(userId.ToString(), HandleSendPushAsync, dto);
                        }
                        else
                        {
                            dto.Title = notificationMessage.PushNotificationTemplate!.TemplateTitle;
                            dto.Body = notificationMessage.PushNotificationTemplate.TemplateBody;
                            logger.LogInformation("Template title {Title}; Template body {Body}; User id: {Id}", dto.Title, dto.Body, userId.ToString());

                            await HandleSendPushAsync(dto, userId.ToString());
                        }
                    }
                }
            }
        }

        private async Task<List<string>> GetDeviceTokens(Guid userId)
        {
            var spec = new GetDeviceTokenByUserSpecification(userId);
            var tokenList = await unitOfWork.Repository<PushNotificationTokens>().GetAllWithSpecificationAsync(spec);

            return tokenList.Select(x => x.DeviceToken).ToList();
        }

        private async Task HandleSendPushAsync(NotificationDto dto, string userId)
        {
            await notificationsStorageService.SaveNotificationAsync(userId.ToString(), dto);

            var tokenList = await GetDeviceTokens(Guid.Parse(userId));
            await pushNotificationService.SendPushNotificationAsync(
                tokenList,
                dto);
        }
    }
}