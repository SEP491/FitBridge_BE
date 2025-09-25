using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Enums.Templates;

namespace FitBridge_Domain.Entities.MessageAndReview;

public class Template : BaseEntity
{
    public EnumContentType ContentType { get; set; }
    public TemplateCategory Category { get; set; }
    public string TemplateTile { get; set; }
    public string TemplateBody { get; set; } = null!;
    public Notification InAppNotification { get; set; } = new Notification();
}