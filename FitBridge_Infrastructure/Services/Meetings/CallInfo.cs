using System.Text.Json.Serialization;

namespace FitBridge_Infrastructure.Services.Meetings
{
    public class CallInfo
    {
        [JsonInclude]
        public HashSet<string> ConnectedConnectionIds { get; set; } = new();

        [JsonInclude]
        public Dictionary<string, object>? CallDetails { get; set; }

        public CallInfo()
        {
        }

        [JsonConstructor]
        public CallInfo(HashSet<string> connectedConnectionIds, Dictionary<string, object>? callDetails)
        {
            ConnectedConnectionIds = connectedConnectionIds;
            CallDetails = callDetails;
        }
    }
}