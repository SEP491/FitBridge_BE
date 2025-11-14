using MediatR;

namespace FitBridge_Application.Features.Categories.UpdateSubCategory
{
    public class UpdateSubCategoryCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
    }
}
