using System.Collections.Concurrent;

namespace FitBridge_Application.Interfaces.Services.Notifications
{
    public interface INotificationConnectionManager
    {
        /// <summary>
        /// Adds one or more connection values to the specified key.
        /// Implementation can choose data structure (List for order, Set for uniqueness, etc.)
        /// </summary>
        Task AddConnectionAsync(string keyId, params string[] valueIds);

        /// <summary>
        /// Removes all connections associated with the specified key.
        /// </summary>
        Task<bool> RemoveConnectionAsync(string keyId);

        /// <summary>
        /// Removes specific connection values from the specified key.
        /// </summary>
        Task<bool> RemoveConnectionAsync(string keyId, params string[] valueIds);

        /// <summary>
        /// Check if connection exists.
        /// </summary>
        Task<bool> IsConnectionExists(string keyId);

        /// <summary>
        /// Get list of connection id by userId.
        /// </summary>
        Task<List<string>> GetConnectionsAsync(string keyId);
    }
}