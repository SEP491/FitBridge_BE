using FirebaseAdmin.Messaging;
using FitBridge_Application.Dtos.Notifications;
using Microsoft.Extensions.Logging;

namespace FitBridge_Infrastructure.Services.Notifications.Helpers
{
    internal class PushNotificationService(
        ILogger<PushNotificationService> logger,
        FirebaseService firebaseService)
    {
        public async Task<List<string>> SendPushNotificationAsync(List<string> userDeviceTokens, NotificationDto notificationDto)
        {
            var firebaseApp = FirebaseMessaging.GetMessaging(firebaseService.GetApp());
            var invalidTokens = new List<string>();
            var tasks = new List<Task>();

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
                        catch (FirebaseMessagingException ex)
                        {
                            if (ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument ||
                                ex.MessagingErrorCode == MessagingErrorCode.Unregistered ||
                                ex.MessagingErrorCode == MessagingErrorCode.SenderIdMismatch)
                            {
                                logger.LogWarning("[FCM] Invalid device token detected: {DeviceToken}, ErrorCode: {ErrorCode}",
                                    deviceToken, ex.MessagingErrorCode);
                                lock (invalidTokens)
                                {
                                    invalidTokens.Add(deviceToken);
                                }
                            }
                            else
                            {
                                logger.LogError("[FCM] Failed to send push notification: {DeviceToken}, {Exception}",
                                    deviceToken, ex);
                            }
                        }
                    }
                ));
            }

            await Task.WhenAll(tasks);
            return invalidTokens; // Return list of invalid tokens
        }
    }
}