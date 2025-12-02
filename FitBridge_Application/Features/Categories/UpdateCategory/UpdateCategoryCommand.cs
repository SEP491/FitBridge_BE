using MediatR;

namespace FitBridge_Application.Features.Categories.UpdateCategory
{
    public class UpdateCategoryCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
