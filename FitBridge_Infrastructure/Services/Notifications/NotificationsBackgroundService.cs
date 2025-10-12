using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Services.Notifications.UserNotifications;
using FitBridge_Application.Specifications.Notifications;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Infrastructure.Services.Notifications.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
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
        NotificationConnectionManager notificationConnectionManager,
        PushNotificationService pushNotificationService,
        NotificationHandshakeManager notificationHandshakeManager,
        IServiceScopeFactory serviceScopeFactory) : BackgroundService
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

                            var connectionIds = await notificationConnectionManager.GetConnectionsAsync(userId.ToString());
                            await hubContext.Clients.Clients(connectionIds.ToList()).NotificationReceived();

                            var handshakeContext = new HandshakeContext
                            {
                                Callback = HandleSendPushAsync,
                                NotificationDto = dto,
                                NotificationMessage = notificationMessage
                            };

                            notificationHandshakeManager.StartHandshake(userId.ToString(), handshakeContext);
                        }
                        else
                        {
                            dto.Title = notificationMessage.PushNotificationTemplate!.TemplateTitle;
                            dto.Body = notificationMessage.PushNotificationTemplate.TemplateBody;
                            logger.LogInformation("Template title {Title}; Template body {Body}; User id: {Id}", dto.Title, dto.Body, userId.ToString());
                            await HandleSendPushAsync(dto, notificationMessage, userId.ToString());
                        }
                    }
                }
            }
        }

        private async Task<List<PushNotificationTokens>> GetDeviceTokens(Guid userId)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var spec = new GetDeviceTokenByUserSpecification(userId);
            var tokenList = await unitOfWork.Repository<PushNotificationTokens>().GetAllWithSpecificationAsync(spec);

            return tokenList.ToList();
        }

        private async Task HandleSendPushAsync(NotificationDto dto, NotificationMessage notificationMessage, string userId)
        {
            logger.LogInformation("Initiate push");
            dto.Title = notificationMessage.PushNotificationTemplate!.TemplateTitle;
            dto.Body = notificationMessage.PushNotificationTemplate.TemplateBody;
            logger.LogInformation("Template title {Title}; Template body {Body}; User id: {Id}", dto.Title, dto.Body, userId.ToString());

            var tokenList = await GetDeviceTokens(Guid.Parse(userId));
            var invalidTokens = await pushNotificationService.SendPushNotificationAsync(
                tokenList.AsReadOnly(),
                dto);

            await RemoveInvalidTokens(invalidTokens);
        }

        private async Task RemoveInvalidTokens(IReadOnlyList<Guid> invalidTokens)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            foreach (var tokenId in invalidTokens)
            {
                unitOfWork.Repository<PushNotificationTokens>().Delete(tokenId);
            }

            await unitOfWork.CommitAsync();
        }
    }
}