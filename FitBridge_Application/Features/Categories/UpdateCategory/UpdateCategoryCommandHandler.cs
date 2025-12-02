using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Categories.GetCategoryByName;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Categories.UpdateCategory
{
    internal class UpdateCategoryCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateCategoryCommand, bool>
    {
        public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await unitOfWork.Repository<Category>().GetByIdAsync(request.Id, asNoTracking: false)
                ?? throw new NotFoundException(nameof(Category));

            // Check if another category with the same name exists (excluding current category)
            var spec = new GetCategoryByNameSpec(request.Name);
            var existingCategory = await unitOfWork.Repository<Category>()
                .GetBySpecificationAsync(spec);
            
            if (existingCategory != null && existingCategory.Id != request.Id)
            {
                throw new DataValidationFailedException($"Category with name '{request.Name}' already exists.");
            }

            category.Name = request.Name;
            category.UpdatedAt = DateTime.UtcNow;
            
            unitOfWork.Repository<Category>().Update(category);
            await unitOfWork.CommitAsync();
            
            return true;
        }
    }
}
