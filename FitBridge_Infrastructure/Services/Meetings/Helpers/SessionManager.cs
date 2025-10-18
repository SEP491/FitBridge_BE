using FitBridge_Application.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NRedisStack.Json.DataTypes;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FitBridge_Infrastructure.Services.Meetings.Helpers
{
    public class SessionManager(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisSettings> redisSettings,
        ILogger<SessionManager> logger)
    {
        private readonly IDatabase _database = connectionMultiplexer.GetDatabase(redisSettings.Value.DefaultStorage);

        private readonly ILogger<SessionManager> _logger = logger;

        private readonly string _callInfoKeyPrefix = redisSettings.Value.MeetingCallInfoKeyPrefix;

        private readonly TimeSpan _sessionExpiration = TimeSpan.FromSeconds(redisSettings.Value.MeetingSessionExpirationSeconds);

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

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
                var json = _database.JSON();
                var callInfo = await json.GetAsync<CallInfo>(callInfoKey);

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
                var json = _database.JSON();

                await json.SetAsync(callInfoKey, "$", callInfo);
                await _database.KeyExpireAsync(callInfoKey, _sessionExpiration);

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

                var callInfoKey = GetCallInfoKey(roomId);

                // Check if room exists
                var roomExists = await _database.KeyExistsAsync(callInfoKey);
                if (!roomExists)
                {
                    _logger.LogWarning("Attempted to add connection to non-existent room {RoomId}", roomId);
                    return;
                }

                var json = _database.JSON();

                // Add connection to the array using JSON.ARRAPPEND
                await json.ArrAppendAsync(callInfoKey, "$.ConnectedConnectionIds", connectionId);

                // Refresh expiration
                await _database.KeyExpireAsync(callInfoKey, _sessionExpiration);

                // Get the updated array length for logging
                var lengths = await json.ArrLenAsync(callInfoKey, "$.ConnectedConnectionIds");
                var connectionCount = lengths?.FirstOrDefault() ?? 0;

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

                var callInfoKey = GetCallInfoKey(roomId);

                // Check if room exists
                var roomExists = await _database.KeyExistsAsync(callInfoKey);
                if (!roomExists)
                {
                    _logger.LogWarning("Attempted to remove connection from non-existent room {RoomId}", roomId);
                    return;
                }

                var json = _database.JSON();

                // Get current connections array
                var connections = await json.GetAsync<List<string>>(callInfoKey, "$.ConnectedConnectionIds");
                if (connections != null && connections.Count > 0)
                {
                    var index = connections.IndexOf(connectionId);

                    if (index >= 0)
                    {
                        // Remove the element at the found index using JSON.ARRPOP
                        await json.ArrPopAsync(callInfoKey, "$.ConnectedConnectionIds", index);

                        // Refresh expiration
                        await _database.KeyExpireAsync(callInfoKey, _sessionExpiration);

                        // Get the updated array length for logging
                        var lengths = await json.ArrLenAsync(callInfoKey, "$.ConnectedConnectionIds");
                        var remainingCount = lengths?.FirstOrDefault() ?? 0;

                        _logger.LogInformation("Removed connection {ConnectionId} from room {RoomId}. ConnectedCount={ConnectedCount}",
                            connectionId, roomId, remainingCount);
                    }
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

                // Get connection count before deletion
                var callInfo = await GetCallInfoAsync(roomId);
                var connectionCount = callInfo?.ConnectedConnectionIds.Count ?? 0;

                // Delete the key
                await _database.KeyDeleteAsync(callInfoKey);

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

                var callInfo = await GetCallInfoAsync(roomId);
                return callInfo?.ConnectedConnectionIds.Contains(connectionId) ?? false;
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

                var callInfo = await GetCallInfoAsync(roomId);
                return callInfo?.ConnectedConnectionIds.Count ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting connection count for room {RoomId}", roomId);
                return 0;
            }
        }
    }
}