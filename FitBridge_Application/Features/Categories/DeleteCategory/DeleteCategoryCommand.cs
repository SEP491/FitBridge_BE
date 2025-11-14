using MediatR;

namespace FitBridge_Application.Features.Categories.DeleteCategory
{
    public class DeleteCategoryCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
