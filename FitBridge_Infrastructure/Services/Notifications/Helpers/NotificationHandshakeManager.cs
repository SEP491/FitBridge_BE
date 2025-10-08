using FitBridge_Application.Configurations;
using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Interfaces.Services.Notifications.UserNotifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace FitBridge_Infrastructure.Services.Notifications.Helpers
{
    public class NotificationHandshakeManager(
        IOptions<NotificationSettings> notificationSettings,
        IHubContext<NotificationHub, IUserNotifications> hubContext,
        ILogger<NotificationHandshakeManager> logger)
    {
        private NotificationSettings settings = notificationSettings.Value;

        private static readonly ConcurrentDictionary<string, HandshakeState> handshakeStates = [];

        internal void StartHandshake(string userId, HandshakeContext handshakeContext)
        {
            var state = new HandshakeState { RetryCount = 0 };
            handshakeStates.TryAdd(userId, state);

            _ = RetryHandshakeAsync(userId, state, handshakeContext);
        }

        private async Task RetryHandshakeAsync(string userId,
            HandshakeState state, HandshakeContext handshakeContext)
        {
            while (state.RetryCount < settings.MaxHandshakeRetries)
            {
                try
                {
                    logger.LogInformation("Retry handshake: {Count}", state.RetryCount);
                    await hubContext.Clients.User(userId).NotificationReceived();

                    state.CancellationTokenSource = new CancellationTokenSource(settings.InitialRetryDelayMs);

                    await Task.Delay(settings.InitialRetryDelayMs, state.CancellationTokenSource.Token);

                    state.RetryCount++;
                }
                catch (TaskCanceledException ex)
                {
                    logger.LogInformation(ex, "Handshake completes");
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Handshake fails, reasons {Exception}, retrying", ex);
                    break;
                }
            }

            if (state.RetryCount >= settings.MaxHandshakeRetries)
            {
                handshakeStates.TryRemove(userId, out _);
                await handshakeContext.Callback.Invoke(
                    handshakeContext.NotificationDto,
                    handshakeContext.NotificationMessage,
                    userId);
            }
        }

        internal Task ConfirmHandshake(string userId)
        {
            if (handshakeStates.TryGetValue(userId, out var state))
            {
                logger.LogInformation("Client {UserId} confirmed handshake", userId);
                state.CancellationTokenSource?.Cancel();
                handshakeStates.TryRemove(userId, out _);
            }
            return Task.CompletedTask;
        }
    }
}