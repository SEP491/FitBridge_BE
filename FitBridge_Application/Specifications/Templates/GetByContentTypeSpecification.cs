using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Enums.Templates;

namespace FitBridge_Application.Specifications.Templates
{
    public class GetByTemplateTypeSpecification : BaseSpecification<Template>
    {
        public GetByTemplateTypeSpecification(EnumContentType contentType, TemplateCategory templateCategory) : base(x =>
            x.IsEnabled && x.ContentType == contentType && templateCategory == x.Category
        )
        {
        }
    }
}