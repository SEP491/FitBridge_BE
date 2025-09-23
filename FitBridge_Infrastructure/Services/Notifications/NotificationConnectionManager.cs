using Castle.Core.Logging;
using FitBridge_Application.Interfaces.Services.Notifications;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Collections.Concurrent;
using System.Linq;

namespace FitBridge_Infrastructure.Services.Notifications
{
    internal class NotificationConnectionManager(ILogger<NotificationConnectionManager> logger) : INotificationConnectionManager
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

        public Task<List<string>> GetConnectionsAsync(string keyId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsConnectionExists(string keyId)
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

            var list = await GetConnectionsAsync(keyId);
            var updated = false;
            if (list.Count > 0)
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
                    logger.Error(ex.Message);
                }
            }
            return updated;
        }
    }
}