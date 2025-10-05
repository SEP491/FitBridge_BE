using System.Collections.Concurrent;

namespace FitBridge_Infrastructure.Services.Meetings
{
    internal class CallInfo
    {
        public ConcurrentDictionary<string, byte> ConnectedConnectionIds { get; } = new();

        public Dictionary<string, object>? CallDetails { get; set; }
    }
}