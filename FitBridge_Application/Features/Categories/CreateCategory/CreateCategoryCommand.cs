using FitBridge_Application.Dtos.Categories;
using MediatR;

namespace FitBridge_Application.Features.Categories.CreateCategory
{
    public class CreateCategoryCommand : IRequest<CreateCategoryResponseDto>
    {
        public string Name { get; set; }
    }
}
