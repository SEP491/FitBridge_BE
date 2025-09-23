using FitBridge_Application.Configurations;
using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Interfaces.Services.Notifications;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace FitBridge_Infrastructure.Services.Notifications
{
    internal class NotificationStorageService(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisSettings> redisSettings,
        IOptions<NotificationSettings> notificationSettings
        ) : INotificationsStorageService
    {
        private readonly IDatabase database = connectionMultiplexer.GetDatabase(
            redisSettings.Value.NotificationStorage);

        public Task ClearAllNotificationsAsync(string userId)
        {
            return database.KeyDeleteAsync(userId);
        }

        public async Task<List<NotificationDto>> GetNotificationsAsync(string userId)
        {
            var notifications = await database.SetMembersAsync(userId);
            return notifications.Select(n => JsonSerializer.Deserialize<NotificationDto>(n!)!).ToList();
        }

        public async Task SaveNotificationAsync(string userId, NotificationDto notificationDto)
        {
            await database.SetAddAsync(userId, JsonSerializer.Serialize(notificationDto));
        }
    }
}