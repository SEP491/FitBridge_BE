using FitBridge_Domain.Enums.MessageAndReview;

namespace FitBridge_Application.Dtos.Templates
{
    public class TemplateDto
    {
        public EnumContentType ContentType { get; set; }

        public TemplateCategory Category { get; set; }

        public string TemplateBody { get; set; } = null!;
    }
}