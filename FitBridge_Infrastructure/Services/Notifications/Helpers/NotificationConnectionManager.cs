using FitBridge_Application.Interfaces.Services.Notifications;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Linq;

namespace FitBridge_Infrastructure.Services.Notifications.Helpers
{
    public class NotificationConnectionManager(ILogger<NotificationConnectionManager> logger)
    {
        private readonly ConcurrentDictionary<string, HashSet<string>> connections = new();

        public Task AddConnectionAsync(string keyId, params string[] valueIds)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(keyId);
            if (valueIds.Length == 0 || valueIds is null)
            {
                throw new ArgumentException("ValueIds cannot be null or empty.");
            }

            connections.AddOrUpdate(keyId,
                key => valueIds.ToHashSet(),
                (key, existingList) =>
                {
                    lock (existingList)
                    {
                        foreach (var id in valueIds.Where(id => !string.IsNullOrEmpty(id)))
                        {
                            existingList.Add(id);
                        }
                    }
                    return existingList;
                });

            return Task.CompletedTask;
        }

        public Task<bool> IsConnectionExistsAsync(string keyId)
        {
            connections.TryGetValue(keyId, out var valueIds);
            return valueIds!.Count > 0 ? Task.FromResult(true) : Task.FromResult(false);
        }

        public Task<bool> RemoveConnectionAsync(string keyId)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(keyId);
            return Task.FromResult(connections.TryRemove(keyId, out _));
        }

        public async Task<bool> RemoveConnectionAsync(string keyId, params string[] valueIds)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(keyId);
            if (valueIds.Length == 0 || valueIds is null)
            {
                throw new ArgumentException("ValueIds cannot be null or empty.");
            }

            var hashSet = GetConnections(keyId);
            var updated = false;
            if (hashSet.Count > 0)
            {
                try
                {
                    connections.AddOrUpdate(keyId,
                        key => new HashSet<string>(),
                        (key, existingList) =>
                        {
                            lock (existingList)
                            {
                                existingList.RemoveWhere(x => valueIds.Contains(x));
                            }
                            updated = true;
                            return existingList;
                        });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
            }
            return updated;
        }

        public HashSet<string> GetConnections(string keyId)
        {
            connections.TryGetValue(keyId, out HashSet<string>? hashSet);

            ArgumentNullException.ThrowIfNull(hashSet);
            return hashSet;
        }
    }
}