using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Categories.DeleteCategory
{
    internal class DeleteCategoryCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteCategoryCommand, bool>
    {
        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await unitOfWork.Repository<Category>().GetByIdAsync(request.Id, asNoTracking: false)
                ?? throw new NotFoundException(nameof(Category));

            // Soft delete the category
            unitOfWork.Repository<Category>().SoftDelete(category);
            
            await unitOfWork.CommitAsync();
            
            return true;
        }
    }
}
