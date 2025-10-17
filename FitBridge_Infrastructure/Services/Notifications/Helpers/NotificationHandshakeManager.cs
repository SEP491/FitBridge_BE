using FitBridge_Application.Configurations;
using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Interfaces.Services.Notifications.UserNotifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace FitBridge_Infrastructure.Services.Notifications.Helpers
{
    public class NotificationHandshakeManager(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisSettings> redisSettings,
        IOptions<NotificationSettings> notificationSettings,
        IHubContext<NotificationHub, IUserNotifications> hubContext,
        ILogger<NotificationHandshakeManager> logger)
    {
        private readonly NotificationSettings settings = notificationSettings.Value;
        private readonly IDatabase database = connectionMultiplexer.GetDatabase(redisSettings.Value.DefaultStorage);
        private readonly string KeyPrefix = redisSettings.Value.NotificationHandshakeKeyPrefix;
        private readonly TimeSpan KeyExpiration = TimeSpan.FromSeconds(redisSettings.Value.ConnectionKeyExpirationSeconds);

        private string GetRedisKey(string userId) => $"{KeyPrefix}{userId}";

        internal async void StartHandshake(string userId, HandshakeContext handshakeContext)
        {
            try
            {
                var state = new HandshakeState { RetryCount = 0, StartedAt = DateTime.UtcNow };
                
                // Store initial state in Redis
                await SetHandshakeStateAsync(userId, state);

                _ = RetryHandshakeAsync(userId, state, handshakeContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error starting handshake for user {UserId}", userId);
            }
        }

        private async Task RetryHandshakeAsync(string userId,
            HandshakeState state, HandshakeContext handshakeContext)
        {
            while (state.RetryCount < settings.MaxHandshakeRetries)
            {
                try
                {
                    logger.LogInformation("Retry handshake attempt {Count} for user {UserId}", state.RetryCount, userId);

                    state.CancellationTokenSource?.Dispose();
                    state.CancellationTokenSource = new CancellationTokenSource();

                    await Task.Delay(settings.InitialRetryDelayMs, state.CancellationTokenSource.Token);

                    state.RetryCount++;
                    
                    // Update state in Redis
                    await SetHandshakeStateAsync(userId, state);

                    await hubContext.Clients.User(userId).NotificationReceived();
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

                await RemoveHandshakeStateAsync(userId);
                state.Dispose();

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

        internal async Task ConfirmHandshake(string userId)
        {
            try
            {
                var stateData = await GetHandshakeStateAsync(userId);
                
                if (stateData != null)
                {
                    logger.LogInformation("Client {UserId} confirmed handshake", userId);

                    // Remove from Redis
                    await RemoveHandshakeStateAsync(userId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error confirming handshake for user {UserId}", userId);
            }
        }

        private async Task SetHandshakeStateAsync(string userId, HandshakeState state)
        {
            try
            {
                var redisKey = GetRedisKey(userId);
                var stateData = new HandshakeStateData
                {
                    RetryCount = state.RetryCount,
                    StartedAt = state.StartedAt
                };

                var serializedData = JsonSerializer.Serialize(stateData);
                await database.StringSetAsync(redisKey, serializedData, KeyExpiration);

                logger.LogDebug("Stored handshake state for user {UserId} with retry count {RetryCount}", 
                    userId, state.RetryCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error storing handshake state for user {UserId}", userId);
                throw;
            }
        }

        private async Task<HandshakeStateData?> GetHandshakeStateAsync(string userId)
        {
            try
            {
                var redisKey = GetRedisKey(userId);
                var serializedData = await database.StringGetAsync(redisKey);

                if (serializedData.IsNullOrEmpty)
                {
                    return null;
                }

                return JsonSerializer.Deserialize<HandshakeStateData>(serializedData.ToString());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving handshake state for user {UserId}", userId);
                return null;
            }
        }

        private async Task<bool> RemoveHandshakeStateAsync(string userId)
        {
            try
            {
                var redisKey = GetRedisKey(userId);
                var deleted = await database.KeyDeleteAsync(redisKey);

                if (deleted)
                {
                    logger.LogInformation("Removed handshake state for user {UserId}", userId);
                }

                return deleted;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error removing handshake state for user {UserId}", userId);
                return false;
            }
        }
    }
}