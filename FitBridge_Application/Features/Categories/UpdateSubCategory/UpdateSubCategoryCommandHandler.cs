using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Categories.GetSubCategoryByNameAndCategoryId;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Categories.UpdateSubCategory
{
    internal class UpdateSubCategoryCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateSubCategoryCommand, bool>
    {
        public async Task<bool> Handle(UpdateSubCategoryCommand request, CancellationToken cancellationToken)
        {
            var subCategory = await unitOfWork.Repository<SubCategory>().GetByIdAsync(request.Id, asNoTracking: false)
                ?? throw new NotFoundException(nameof(SubCategory));

            // Verify that the category exists
            var category = await unitOfWork.Repository<Category>().GetByIdAsync(request.CategoryId)
                ?? throw new NotFoundException($"Category with ID '{request.CategoryId}' not found.");

            // Check if another subcategory with the same name exists in this category (excluding current subcategory)
            var spec = new GetSubCategoryByNameAndCategoryIdSpec(request.Name, request.CategoryId);
            var existingSubCategory = await unitOfWork.Repository<SubCategory>()
                .GetBySpecificationAsync(spec);

            if (existingSubCategory != null && existingSubCategory.Id != request.Id)
            {
                throw new InvalidDataException($"SubCategory with name '{request.Name}' already exists in this category.");
            }

            subCategory.Name = request.Name;
            subCategory.CategoryId = request.CategoryId;
            subCategory.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Repository<SubCategory>().Update(subCategory);
            await unitOfWork.CommitAsync();

            return true;
        }
    }
}