using System.ComponentModel.DataAnnotations;

namespace FitBridge_Application.Configurations
{
    public class RedisSettings
    {
        public const string SectionName = "Redis";

        [Required]
        public string ConnectionString { get; set; } = string.Empty;

        [Required]
        public int NotificationStorage { get; set; } = 1;

        [Required]
        public string NotificationKeyPrefix { get; set; } = "notification:connections:";

        /// <summary>
        /// Expiration time for notification connection keys in seconds. Default is 1 day (86400 seconds).
        /// </summary>
        public int ConnectionKeyExpirationSeconds { get; set; } = 86400; // 1 day
    }
}