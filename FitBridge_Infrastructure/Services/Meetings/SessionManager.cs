using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace FitBridge_Infrastructure.Services.Meetings
{
    internal class SessionManager(ILogger<SessionManager> logger)
    {
        private readonly ConcurrentDictionary<string, CallInfo> Calls = new();

        public CallInfo? GetCallInfo(string roomId)
        {
            if (Calls.TryGetValue(roomId, out var callInfo))
            {
                return callInfo;
            }
            return null;
        }

        public void SetCallInfo(string roomId, CallInfo callInfo)
        {
            if (callInfo == null)
            {
                logger.LogWarning("Attempted to set null CallInfo for connection {RoomId}", roomId);
                return;
            }
            Calls.GetOrAdd(roomId, callInfo);
            logger.LogInformation("Set CallInfo for connection {RoomId}: ConnectedCount={ConnectedCount}",
                roomId, callInfo.ConnectedConnectionIds.Count);
        }

        public void AddConnectionToRoom(string roomId, string connectionId)
        {
            if (!Calls.TryGetValue(roomId, out var callInfo))
            {
                logger.LogWarning("Attempted to add connection to non-existent room {RoomId}", roomId);
                return;
            }
            callInfo.ConnectedConnectionIds.TryAdd(connectionId, 0);
            logger.LogInformation("Added connection {ConnectionId} to room {RoomId}. ConnectedCount={ConnectedCount}",
                connectionId, roomId, callInfo.ConnectedConnectionIds.Count);
        }

        public void RemoveConnectionFromRoom(string roomId, string connectionId)
        {
            if (!Calls.TryGetValue(roomId, out var callInfo))
            {
                logger.LogWarning("Attempted to remove connection from non-existent room {RoomId}", roomId);
                return;
            }
            callInfo.ConnectedConnectionIds.TryRemove(connectionId, out _);
            logger.LogInformation("Removed connection {ConnectionId} from room {RoomId}. ConnectedCount={ConnectedCount}",
                connectionId, roomId, callInfo.ConnectedConnectionIds.Count);
        }

        public void RemoveCallInfo(string roomId)
        {
            if (Calls.TryRemove(roomId, out var callInfo))
            {
                logger.LogInformation("Removed CallInfo for room {RoomId}: ConnectedCount={ConnectedCount}",
                    roomId, callInfo.ConnectedConnectionIds.Count);
            }
            else
            {
                logger.LogWarning("No CallInfo found for connection {RoomId} to remove", roomId);
            }
        }

        public IEnumerable<KeyValuePair<string, CallInfo>> GetAllCallInfo()
        {
            return Calls;
        }
    }
}