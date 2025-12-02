using MediatR;

namespace FitBridge_Application.Features.Categories.DeleteSubCategory
{
    public class DeleteSubCategoryCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
