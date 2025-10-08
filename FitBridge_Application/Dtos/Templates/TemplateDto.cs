using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Enums.Templates;

namespace FitBridge_Application.Dtos.Templates
{
    public class TemplateDto
    {
        public Guid Id { get; set; }

        public EnumContentType ContentType { get; set; }

        public TemplateCategory Category { get; set; }

        public string TemplateTitle { get; set; }

        public string TemplateBody { get; set; }
    }
}