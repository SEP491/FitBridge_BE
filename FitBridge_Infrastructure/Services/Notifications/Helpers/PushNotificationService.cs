using dotAPNS;
using dotAPNS.AspNetCore;
using FirebaseAdmin.Messaging;
using FitBridge_Application.Configurations;
using FitBridge_Application.Dtos.Notifications;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Enums.Notifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FitBridge_Infrastructure.Services.Notifications.Helpers
{
    internal class PushNotificationService(
        ILogger<PushNotificationService> logger,
        IApnsClientFactory apnsClientFactory,
        IApnsService apnsService,
        IOptions<NotificationSettings> notificationSettings,
        FirebaseService firebaseService)
    {
        public async Task<IReadOnlyList<Guid>> SendPushNotificationAsync(
            IReadOnlyList<PushNotificationTokens> userDeviceTokens, NotificationDto notificationDto)
        {
            List<Guid> failedTokens = [];
            foreach (var userDeviceToken in userDeviceTokens)
            {
                try
                {
                    switch (userDeviceToken.Platform)
                    {
                        case PlatformEnum.Android:
                            await SendAndroidPushAsync(userDeviceToken, notificationDto);
                            break;

                        case PlatformEnum.iOS:
                            await SendIOSPushAsync(userDeviceToken, notificationDto);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(userDeviceToken.Platform), userDeviceToken.Platform, "Unsupported platform type");
                    }
                }
                catch (FirebaseMessagingException)
                {
                    failedTokens.Add(userDeviceToken.Id);
                }
                catch (ApnsCertificateExpiredException)
                {
                    failedTokens.Add(userDeviceToken.Id);
                }
            }

            return failedTokens.AsReadOnly();
        }

        private async Task SendIOSPushAsync(
            PushNotificationTokens userDeviceToken, NotificationDto notificationDto)
        {
            var options = new ApnsJwtOptions
            {
                TeamId = notificationSettings.Value.IOS_TeamId,
                KeyId = notificationSettings.Value.IOS_KeyId,
                BundleId = notificationSettings.Value.IOS_BundleId,
                CertFilePath = notificationSettings.Value.IOS_CertificatePath
            };

            var client = apnsClientFactory.CreateUsingJwt(options);
            var push = new ApplePush(ApplePushType.Alert)
                .AddAlert(notificationDto.Title, notificationDto.Body)
                .AddToken(userDeviceToken.DeviceToken);

            try
            {
                var response = await client.SendAsync(push);
                if (response.IsSuccessful)
                {
                    logger.LogInformation("An alert push has been successfully sent!");
                }
                else
                {
                    switch (response.Reason)
                    {
                        case ApnsResponseReason.BadCertificateEnvironment:
                            logger.LogInformation("The certificate is for the wrong environment (e.g. using a sandbox certificate with the production service).");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(response.Reason), response.Reason, null);
                    }
                    logger.LogInformation("Failed to send a push, APNs reported an error: {Reason}", response.ReasonString);
                }
            }
            catch (TaskCanceledException ex)
            {
                logger.LogWarning(ex, "Failed to send a push: HTTP request timed out.");
                throw;
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "Failed to send a push. HTTP request failed: " + ex);
                throw;
            }
            catch (ApnsCertificateExpiredException)
            {
                logger.LogWarning("APNs certificate has expired. No more push notifications can be sent using it until it is replaced with a new one.");
                throw;
            }
        }

        private async Task SendAndroidPushAsync(
            PushNotificationTokens userDeviceToken, NotificationDto notificationDto)
        {
            var firebaseApp = FirebaseMessaging.GetMessaging(firebaseService.GetApp());

            var message = new FirebaseAdmin.Messaging.Message
            {
                Token = userDeviceToken.DeviceToken,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = notificationDto.Title,
                    Body = notificationDto.Body
                },
            };
            try
            {
                await firebaseApp.SendAsync(message);
                logger.LogInformation("Successfully sent message to {DeviceToken}", userDeviceToken.DeviceToken);
            }
            catch (FirebaseMessagingException ex)
            {
                if (ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument ||
                    ex.MessagingErrorCode == MessagingErrorCode.Unregistered ||
                    ex.MessagingErrorCode == MessagingErrorCode.SenderIdMismatch)
                {
                    logger.LogWarning("[FCM] Invalid device token detected: {DeviceToken}, ErrorCode: {ErrorCode}",
                        userDeviceToken, ex.MessagingErrorCode);
                }
                else
                {
                    logger.LogError(ex, "[FCM] Failed to send push notification: {DeviceToken}, {Exception}",
                        userDeviceToken, ex);
                }
                throw;
            }
        }
    }
}