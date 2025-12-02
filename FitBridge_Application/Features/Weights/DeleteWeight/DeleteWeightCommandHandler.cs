using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Weights.DeleteWeight
{
    internal class DeleteWeightCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteWeightCommand, bool>
    {
        public async Task<bool> Handle(DeleteWeightCommand request, CancellationToken cancellationToken)
        {
            var weight = await unitOfWork.Repository<Weight>().GetByIdAsync(request.Id, asNoTracking: false)
                ?? throw new NotFoundException(nameof(Weight));

            // Soft delete the weight
            unitOfWork.Repository<Weight>().SoftDelete(weight);

            await unitOfWork.CommitAsync();

            return true;
        }
    }
}