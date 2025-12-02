using FitBridge_Application.Dtos.Categories;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Categories.GetCategoryByName;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Categories.CreateCategory
{
    internal class CreateCategoryCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<CreateCategoryCommand, CreateCategoryResponseDto>
    {
        public async Task<CreateCategoryResponseDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var spec = new GetCategoryByNameSpec(request.Name);
            var category = await unitOfWork.Repository<Category>()
                .GetBySpecificationAsync(spec);

            if (category != null)
            {
                throw new DataValidationFailedException($"Category with name '{request.Name}' already exists.");
            }

            var newCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
            };

            unitOfWork.Repository<Category>().Insert(newCategory);

            await unitOfWork.CommitAsync();
            return new CreateCategoryResponseDto { Id = newCategory.Id };
        }
    }
}