using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Enums;

namespace FitBridge_Application.Specifications.Templates
{
    public class GetByTemplateTypeSpecification : BaseSpecification<Template>
    {
        public GetByTemplateTypeSpecification(ContentType contentType) : base(
            x => x.ContentType == contentType.ToString() && x.IsEnabled
        )
        {
        }
    }
}