using FitBridge_Application.Configurations;
using FitBridge_Application.Dtos.Notifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace FitBridge_Infrastructure.Services.Notifications.Helpers
{
    internal class NotificationStorageService(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisSettings> redisSettings,
        ILogger<NotificationStorageService> logger
        )
    {
        private readonly IDatabase database = connectionMultiplexer.GetDatabase(
            redisSettings.Value.NotificationStorage);

        public async Task<bool> ClearAllNotificationsAsync(string userId)
        {
            return await database.KeyDeleteAsync(userId);
        }

        public async Task<List<NotificationDto>> GetNotificationsAsync(string userId)
        {
            var notifications = await database.SetMembersAsync(userId);
            var keyDeleted = await ClearAllNotificationsAsync(userId);
            if (!keyDeleted)
            {
                logger.LogWarning("Error deleting key {Key}", userId);
            }
            return notifications.Select(n => JsonSerializer.Deserialize<NotificationDto>(n!)!).ToList();
        }

        public async Task SaveNotificationAsync(string userId, NotificationDto notificationDto)
        {
            await database.SetAddAsync(userId, JsonSerializer.Serialize(notificationDto));
        }
    }
}