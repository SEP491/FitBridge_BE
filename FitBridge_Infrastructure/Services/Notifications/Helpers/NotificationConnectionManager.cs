using FitBridge_Application.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace FitBridge_Infrastructure.Services.Notifications.Helpers
{
    public class NotificationConnectionManager(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisSettings> redisSettings,
        ILogger<NotificationConnectionManager> logger)
    {
        private readonly IDatabase database = connectionMultiplexer.GetDatabase(
            redisSettings.Value.NotificationStorage);

        private readonly string KeyPrefix = redisSettings.Value.NotificationConnectionsKeyPrefix;

        private readonly TimeSpan KeyExpiration = TimeSpan.FromSeconds(redisSettings.Value.ConnectionKeyExpirationSeconds);

        private string GetRedisKey(string keyId) => $"{KeyPrefix}{keyId}";

        public async Task AddConnectionAsync(string keyId, params string[] valueIds)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(keyId);
            if (valueIds.Length == 0 || valueIds is null)
            {
                throw new ArgumentException("ValueIds cannot be null or empty.");
            }

            try
            {
                var redisKey = GetRedisKey(keyId);
                var validIds = valueIds.Where(id => !string.IsNullOrEmpty(id)).ToArray();

                if (validIds.Length == 0)
                {
                    logger.LogWarning("No valid connection IDs to add for key {KeyId}", keyId);
                    return;
                }

                var redisValues = validIds.Select(id => (RedisValue)id).ToArray();
                await database.SetAddAsync(redisKey, redisValues);

                // Set expiration time for the key
                await database.KeyExpireAsync(redisKey, KeyExpiration);

                logger.LogInformation("Added {Count} connection(s) for user {UserId} with expiration of {Expiration}",
                    validIds.Length, keyId, KeyExpiration);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding connections for key {KeyId}", keyId);
                throw;
            }
        }

        public async Task<bool> IsConnectionExistsAsync(string keyId)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(keyId);

            try
            {
                var redisKey = GetRedisKey(keyId);
                var count = await database.SetLengthAsync(redisKey);
                return count > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking if connection exists for key {KeyId}", keyId);
                return false;
            }
        }

        public async Task<bool> RemoveConnectionAsync(string keyId)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(keyId);

            try
            {
                var redisKey = GetRedisKey(keyId);
                var deleted = await database.KeyDeleteAsync(redisKey);

                if (deleted)
                {
                    logger.LogInformation("Removed all connections for user {UserId}", keyId);
                }

                return deleted;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error removing all connections for key {KeyId}", keyId);
                return false;
            }
        }

        public async Task<bool> RemoveConnectionAsync(string keyId, params string[] valueIds)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(keyId);
            if (valueIds.Length == 0 || valueIds is null)
            {
                throw new ArgumentException("ValueIds cannot be null or empty.");
            }

            try
            {
                var redisKey = GetRedisKey(keyId);
                var validIds = valueIds.Where(id => !string.IsNullOrEmpty(id)).ToArray();

                if (validIds.Length == 0)
                {
                    logger.LogWarning("No valid connection IDs to remove for key {KeyId}", keyId);
                    return false;
                }

                var redisValues = validIds.Select(id => (RedisValue)id).ToArray();
                var removedCount = await database.SetRemoveAsync(redisKey, redisValues);

                // Refresh expiration if there are still connections remaining
                if (removedCount > 0)
                {
                    var remainingCount = await database.SetLengthAsync(redisKey);
                    if (remainingCount > 0)
                    {
                        await database.KeyExpireAsync(redisKey, KeyExpiration);
                    }
                }

                logger.LogInformation("Removed {Count} connection(s) for user {UserId}", removedCount, keyId);

                return removedCount > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error removing specific connections for key {KeyId}", keyId);
                return false;
            }
        }

        public async Task<HashSet<string>> GetConnectionsAsync(string keyId)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(keyId);

            try
            {
                var redisKey = GetRedisKey(keyId);
                var members = await database.SetMembersAsync(redisKey);

                return members.Select(m => m.ToString()).ToHashSet();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting connections for key {KeyId}", keyId);
                return [];
            }
        }
    }
}