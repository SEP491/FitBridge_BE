using System.ComponentModel.DataAnnotations;

namespace FitBridge_Application.Configurations
{
    public class RedisSettings
    {
        public const string SectionName = "Redis";

        [Required]
        public string ConnectionString { get; set; } = string.Empty;

        [Required]
        public int DefaultStorage { get; set; } = 1;

        [Required]
        public string NotificationConnectionsKeyPrefix { get; set; } = "notification:connections:";

        [Required]
        public string NotificationHandshakeKeyPrefix { get; set; } = "notification:handshakes:";

        /// <summary>
        /// Key prefix for meeting room connections
        /// </summary>
        [Required]
        public string MeetingConnectionsKeyPrefix { get; set; } = "meeting:connections:";

        /// <summary>
        /// Key prefix for meeting room call info
        /// </summary>
        [Required]
        public string MeetingCallInfoKeyPrefix { get; set; } = "meeting:callinfo:";

        /// <summary>
        /// Expiration time for notification connection keys in seconds. Default is 1 day (86400 seconds).
        /// </summary>
        public int ConnectionKeyExpirationSeconds { get; set; } = 86400; // 1 day

        /// <summary>
        /// Expiration time for meeting session keys in seconds. Default is 2 hours (7200 seconds).
        /// </summary>
        public int MeetingSessionExpirationSeconds { get; set; } = 7200; // 2 hours
    }
}