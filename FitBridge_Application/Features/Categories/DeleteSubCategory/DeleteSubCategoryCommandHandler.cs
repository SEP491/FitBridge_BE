using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Categories.DeleteSubCategory
{
    internal class DeleteSubCategoryCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteSubCategoryCommand, bool>
    {
        public async Task<bool> Handle(DeleteSubCategoryCommand request, CancellationToken cancellationToken)
        {
            var subCategory = await unitOfWork.Repository<SubCategory>().GetByIdAsync(request.Id, asNoTracking: false)
                ?? throw new NotFoundException(nameof(SubCategory));

            // Soft delete the subcategory
            unitOfWork.Repository<SubCategory>().SoftDelete(subCategory);
            
            await unitOfWork.CommitAsync();
            
            return true;
        }
    }
}
