using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Domain.Entities.MessageAndReview;

public class Notification : BaseEntity
{
    public string Body { get; set; }

    public string Title { get; set; }

    public Guid TemplateId { get; set; }

    public Guid UserId { get; set; }

    public Template Template { get; set; }

    public ApplicationUser User { get; set; }

    public DateTime? ReadAt { get; set; }

    public string? AdditionalPayload { get; set; }
}