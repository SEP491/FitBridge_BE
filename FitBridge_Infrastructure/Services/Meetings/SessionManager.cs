using FitBridge_Application.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace FitBridge_Infrastructure.Services.Meetings
{
    public class SessionManager(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisSettings> redisSettings,
        ILogger<SessionManager> logger)
    {
        private readonly IDatabase _database = connectionMultiplexer.GetDatabase(redisSettings.Value.DefaultStorage);

        private readonly ILogger<SessionManager> _logger = logger;

        private readonly string _connectionsKeyPrefix = redisSettings.Value.MeetingConnectionsKeyPrefix;

        private readonly string _callInfoKeyPrefix = redisSettings.Value.MeetingCallInfoKeyPrefix;

        private readonly TimeSpan _sessionExpiration = TimeSpan.FromSeconds(redisSettings.Value.MeetingSessionExpirationSeconds);

        private string GetConnectionsKey(string roomId) => $"{_connectionsKeyPrefix}{roomId}";

        private string GetCallInfoKey(string roomId) => $"{_callInfoKeyPrefix}{roomId}";

        /// <summary>
        /// Gets call information for a specific room
        /// </summary>
        public async Task<CallInfo?> GetCallInfoAsync(string roomId)
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(roomId);

                var callInfoKey = GetCallInfoKey(roomId);
                var serializedData = await _database.StringGetAsync(callInfoKey);

                if (serializedData.IsNullOrEmpty)
                {
                    return null;
                }

                var callInfo = JsonSerializer.Deserialize<CallInfo>(serializedData.ToString());

                // Load connections from Redis set
                if (callInfo != null)
                {
                    var connectionsKey = GetConnectionsKey(roomId);
                    var connections = await _database.SetMembersAsync(connectionsKey);
                    callInfo.ConnectedConnectionIds = connections.Select(c => c.ToString()).ToHashSet();
                }

                return callInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting CallInfo for room {RoomId}", roomId);
                return null;
            }
        }

        /// <summary>
        /// Sets call information for a specific room
        /// </summary>
        public async Task SetCallInfoAsync(string roomId, CallInfo callInfo)
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(roomId);
                ArgumentNullException.ThrowIfNull(callInfo);

                var callInfoKey = GetCallInfoKey(roomId);
                var connectionsKey = GetConnectionsKey(roomId);

                // Store call info (without connections - they're stored separately)
                var callInfoToStore = new CallInfo
                {
                    ConnectedConnectionIds = new HashSet<string>(),
                    CallDetails = callInfo.CallDetails
                };

                var serializedData = JsonSerializer.Serialize(callInfoToStore);
                await _database.StringSetAsync(callInfoKey, serializedData, _sessionExpiration);

                // Initialize empty connections set
                await _database.KeyExpireAsync(connectionsKey, _sessionExpiration);

                _logger.LogInformation("Set CallInfo for room {RoomId}", roomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting CallInfo for room {RoomId}", roomId);
                throw;
            }
        }

        /// <summary>
        /// Adds a connection to a room
        /// </summary>
        public async Task AddConnectionToRoomAsync(string roomId, string connectionId)
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(roomId);
                ArgumentException.ThrowIfNullOrEmpty(connectionId);

                var connectionsKey = GetConnectionsKey(roomId);
                var callInfoKey = GetCallInfoKey(roomId);

                // Check if room exists
                var roomExists = await _database.KeyExistsAsync(callInfoKey);
                if (!roomExists)
                {
                    _logger.LogWarning("Attempted to add connection to non-existent room {RoomId}", roomId);
                    return;
                }

                // Add connection to set
                await _database.SetAddAsync(connectionsKey, connectionId);

                // Refresh expiration on both keys
                await _database.KeyExpireAsync(connectionsKey, _sessionExpiration);
                await _database.KeyExpireAsync(callInfoKey, _sessionExpiration);

                var connectionCount = await _database.SetLengthAsync(connectionsKey);
                _logger.LogInformation("Added connection {ConnectionId} to room {RoomId}. ConnectedCount={ConnectedCount}",
                    connectionId, roomId, connectionCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding connection {ConnectionId} to room {RoomId}", connectionId, roomId);
                throw;
            }
        }

        /// <summary>
        /// Removes a connection from a room
        /// </summary>
        public async Task RemoveConnectionFromRoomAsync(string roomId, string connectionId)
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(roomId);
                ArgumentException.ThrowIfNullOrEmpty(connectionId);

                var connectionsKey = GetConnectionsKey(roomId);
                var callInfoKey = GetCallInfoKey(roomId);

                // Check if room exists
                var roomExists = await _database.KeyExistsAsync(callInfoKey);
                if (!roomExists)
                {
                    _logger.LogWarning("Attempted to remove connection from non-existent room {RoomId}", roomId);
                    return;
                }

                // Remove connection from set
                var removed = await _database.SetRemoveAsync(connectionsKey, connectionId);

                if (removed)
                {
                    // Check remaining connections
                    var remainingCount = await _database.SetLengthAsync(connectionsKey);

                    if (remainingCount > 0)
                    {
                        // Refresh expiration if there are still connections
                        await _database.KeyExpireAsync(connectionsKey, _sessionExpiration);
                        await _database.KeyExpireAsync(callInfoKey, _sessionExpiration);
                    }

                    _logger.LogInformation("Removed connection {ConnectionId} from room {RoomId}. ConnectedCount={ConnectedCount}",
                        connectionId, roomId, remainingCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing connection {ConnectionId} from room {RoomId}", connectionId, roomId);
                throw;
            }
        }

        /// <summary>
        /// Removes all call information for a room
        /// </summary>
        public async Task RemoveCallInfoAsync(string roomId)
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(roomId);

                var callInfoKey = GetCallInfoKey(roomId);
                var connectionsKey = GetConnectionsKey(roomId);

                // Get connection count before deletion
                var connectionCount = await _database.SetLengthAsync(connectionsKey);

                // Delete both keys
                var batch = _database.CreateBatch();
                var deleteCallInfoTask = batch.KeyDeleteAsync(callInfoKey);
                var deleteConnectionsTask = batch.KeyDeleteAsync(connectionsKey);
                batch.Execute();

                await Task.WhenAll(deleteCallInfoTask, deleteConnectionsTask);

                _logger.LogInformation("Removed CallInfo for room {RoomId}: ConnectedCount={ConnectedCount}",
                    roomId, connectionCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing CallInfo for room {RoomId}", roomId);
                throw;
            }
        }

        /// <summary>
        /// Gets all call information across all rooms
        /// </summary>
        public async Task<IEnumerable<KeyValuePair<string, CallInfo>>> GetAllCallInfoAsync()
        {
            try
            {
                var result = new List<KeyValuePair<string, CallInfo>>();
                var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());

                // Scan for all call info keys
                var pattern = $"{_callInfoKeyPrefix}*";
                var keys = server.Keys(_database.Database, pattern: pattern);

                foreach (var key in keys)
                {
                    var roomId = key.ToString().Replace(_callInfoKeyPrefix, string.Empty);
                    var callInfo = await GetCallInfoAsync(roomId);

                    if (callInfo != null)
                    {
                        result.Add(new KeyValuePair<string, CallInfo>(roomId, callInfo));
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all CallInfo");
                return Enumerable.Empty<KeyValuePair<string, CallInfo>>();
            }
        }

        /// <summary>
        /// Checks if a connection exists in a room
        /// </summary>
        public async Task<bool> IsConnectionInRoomAsync(string roomId, string connectionId)
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(roomId);
                ArgumentException.ThrowIfNullOrEmpty(connectionId);

                var connectionsKey = GetConnectionsKey(roomId);
                return await _database.SetContainsAsync(connectionsKey, connectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if connection {ConnectionId} is in room {RoomId}", connectionId, roomId);
                return false;
            }
        }

        /// <summary>
        /// Gets the count of connections in a room
        /// </summary>
        public async Task<long> GetConnectionCountAsync(string roomId)
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(roomId);

                var connectionsKey = GetConnectionsKey(roomId);
                return await _database.SetLengthAsync(connectionsKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting connection count for room {RoomId}", roomId);
                return 0;
            }
        }
    }
}