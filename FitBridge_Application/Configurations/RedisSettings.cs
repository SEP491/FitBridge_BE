using System.ComponentModel.DataAnnotations;

namespace FitBridge_Application.Configurations
{
    public class RedisSettings
    {
        public const string SectionName = "Redis";

        [Required]
        public string ConnectionString { get; set; } = string.Empty;

        public int NotificationStorage { get; set; } = 1;
    }
}