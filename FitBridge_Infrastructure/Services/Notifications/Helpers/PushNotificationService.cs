using FirebaseAdmin.Messaging;
using FitBridge_Application.Dtos.Notifications;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FitBridge_Infrastructure.Services.Notifications.Helpers
{
    internal class PushNotificationService(
        ILogger<PushNotificationService> logger,
        FirebaseService firebaseService)
    {
        public async Task<bool> SendPushNotificationAsync(List<string> userDeviceTokens, NotificationDto notificationDto)
        {
            var firebaseApp = FirebaseMessaging.GetMessaging(firebaseService.GetApp());
            List<Task> tasks = new List<Task>();
            foreach (var deviceToken in userDeviceTokens)
            {
                var message = new Message
                {
                    Token = deviceToken,
                    Notification = new Notification
                    {
                        Title = notificationDto.Title,
                        Body = notificationDto.Body
                    },
                };

                tasks.Add(
                    Task.Run(async () =>
                    {
                        try
                        {
                            await firebaseApp.SendAsync(message);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError("[FCM] Failed to send push notification to token: {DeviceToken}, {Exception}", deviceToken, ex);
                        }
                    }
                ));
            }

            await Task.WhenAll(tasks);

            return true;
        }
    }
}