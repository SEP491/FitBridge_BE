using FitBridge_Application.Dtos.Categories;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Categories.GetSubCategoryByNameAndCategoryId;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Categories.CreateSubCategory
{
    internal class CreateSubCategoryCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<CreateSubCategoryCommand, CreateSubCategoryResponseDto>
    {
        public async Task<CreateSubCategoryResponseDto> Handle(CreateSubCategoryCommand request, CancellationToken cancellationToken)
        {
            // Verify that the category exists
            var category = await unitOfWork.Repository<Category>().GetByIdAsync(request.CategoryId)
                ?? throw new NotFoundException($"Category with ID '{request.CategoryId}' not found.");

            // Check if a subcategory with the same name already exists in this category
            var spec = new GetSubCategoryByNameAndCategoryIdSpec(request.Name, request.CategoryId);
            var subCategory = await unitOfWork.Repository<SubCategory>()
                .GetBySpecificationAsync(spec);
            
            if (subCategory != null)
            {
                throw new InvalidDataException($"SubCategory with name '{request.Name}' already exists in this category.");
            }

            var newSubCategory = new SubCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                CategoryId = request.CategoryId,
            };

            unitOfWork.Repository<SubCategory>().Insert(newSubCategory);

            await unitOfWork.CommitAsync();
            return new CreateSubCategoryResponseDto { Id = newSubCategory.Id };
        }
    }
}
