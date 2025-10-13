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
        private readonly NotificationSettings settings = notificationSettings.Value;

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
                    logger.LogInformation("Retry handshake attempt {Count} for user {UserId}", state.RetryCount, userId);

                    await hubContext.Clients.User(userId).NotificationReceived();

                    state.CancellationTokenSource?.Dispose();
                    state.CancellationTokenSource = new CancellationTokenSource();

                    await Task.Delay(settings.InitialRetryDelayMs, state.CancellationTokenSource.Token);

                    state.RetryCount++;
                }
                catch (TaskCanceledException)
                {
                    logger.LogInformation("Handshake completed successfully for user {UserId}", userId);
                    break;
                }
                catch (OperationCanceledException)
                {
                    logger.LogInformation("Handshake completed successfully for user {UserId}", userId);
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Handshake failed for user {UserId}, reason: {Message}", userId, ex.Message);
                }
            }

            if (state.RetryCount >= settings.MaxHandshakeRetries)
            {
                logger.LogWarning("Handshake max retries reached for user {UserId}, triggering fallback", userId);

                if (handshakeStates.TryRemove(userId, out _))
                {
                    state.Dispose();
                }

                await handshakeContext.Callback.Invoke( // Trigger push
                    handshakeContext.NotificationDto,
                    handshakeContext.NotificationMessage,
                    userId);
            }
            else
            {
                // Cleanup if handshake was confirmed
                state.Dispose();
            }
        }

        internal Task ConfirmHandshake(string userId)
        {
            if (handshakeStates.TryRemove(userId, out var state))
            {
                logger.LogInformation("Client {UserId} confirmed handshake", userId);

                // Cancel and dispose the state
                state.CancellationTokenSource?.Cancel();
                state.Dispose();
            }
            return Task.CompletedTask;
        }
    }
}