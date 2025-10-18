using System.Text.Json.Serialization;

namespace FitBridge_Infrastructure.Services.Meetings.Helpers
{
    public class CallInfo
    {
        [JsonInclude]
        public List<string> ConnectedConnectionIds { get; set; } = new();

        [JsonInclude]
        public Dictionary<string, object>? CallDetails { get; set; }

        public CallInfo()
        {
        }

        [JsonConstructor]
        public CallInfo(List<string> connectedConnectionIds, Dictionary<string, object>? callDetails)
        {
            ConnectedConnectionIds = connectedConnectionIds;
            CallDetails = callDetails;
        }
    }
}