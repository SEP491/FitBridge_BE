using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Enums.MessageAndReview;

namespace FitBridge_Application.Specifications.Templates
{
    public class GetByTemplateTypeSpecification : BaseSpecification<Template>
    {
        public GetByTemplateTypeSpecification(EnumContentType contentType) : base(
            x => x.ContentType == contentType && x.IsEnabled
        )
        {
        }
    }
}