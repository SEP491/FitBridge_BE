using FitBridge_Application.Configurations;
using FitBridge_Application.Dtos.Notifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using static FitBridge_Infrastructure.Services.Notifications.NotificationsBackgroundService;

namespace FitBridge_Infrastructure.Services.Notifications.Helpers
{
    internal class NotificationHandshakeManager(
        IOptions<NotificationSettings> notificationSettings,
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationHandshakeManager> logger)
    {
        private NotificationSettings settings = notificationSettings.Value;

        private static readonly ConcurrentDictionary<string, HandshakeState> handshakeStates = [];

        internal void StartHandshake(string userId, Func<NotificationDto, string, Task> callback, NotificationDto notificationDto)
        {
            var state = new HandshakeState { RetryCount = 0 };
            handshakeStates.TryAdd(userId, state);

            _ = RetryHandshakeAsync(userId, state, callback, notificationDto);
        }

        private async Task RetryHandshakeAsync(string userId, HandshakeState state, Func<NotificationDto, string, Task> callback, NotificationDto notificationDto)
        {
            while (state.RetryCount < settings.MaxHandshakeRetries)
            {
                try
                {
                    await hubContext.Clients.Client(userId).SendAsync("RequestHandshake");

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
                callback.Invoke(notificationDto, userId);
            }
        }

        internal Task ConfirmHandshake(string userId)
        {
            if (handshakeStates.TryGetValue(userId, out var state))
            {
                state.CancellationTokenSource?.Cancel();
                handshakeStates.TryRemove(userId, out _);
            }
            return Task.CompletedTask;
        }
    }
}