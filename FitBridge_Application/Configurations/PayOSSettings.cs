using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitBridge_Application.Configurations
{
    public class PayOSSettings
    {
        public const string SectionName = "PayOS";

        [Required]
        public string ClientId { get; set; } = string.Empty;

        [Required]
        public string ApiKey { get; set; } = string.Empty;

        [Required]
        public string ChecksumKey { get; set; } = string.Empty;

        [Required]
        public string BaseUrl { get; set; } = "https://api-merchant.payos.vn";

        [Required]
        public string ReturnUrl { get; set; } = string.Empty;

        [Required]
        public string CancelUrl { get; set; } = string.Empty;

        [Required]
        public string MobileReturnUrl { get; set; } = string.Empty;

        [Required]
        public string MobileCancelUrl { get; set; } = string.Empty;

        /// <summary>
        /// Payment expiration time in minutes (default 15 minutes)
        /// </summary>
        public int ExpirationMinutes { get; set; } = 30;

        /// <summary>
        /// Whether to use sandbox environment
        /// </summary>
        public bool IsSandbox { get; set; } = true;
    }
}
