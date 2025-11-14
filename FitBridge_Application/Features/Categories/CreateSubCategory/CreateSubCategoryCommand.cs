using FitBridge_Application.Dtos.Categories;
using MediatR;

namespace FitBridge_Application.Features.Categories.CreateSubCategory
{
    public class CreateSubCategoryCommand : IRequest<CreateSubCategoryResponseDto>
    {
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
    }
}
