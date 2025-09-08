using System.ComponentModel.DataAnnotations;

namespace FitBridge_Domain.Entities
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public bool IsEnabled { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}