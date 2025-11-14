using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Weights.GetWeightByValueAndUnit;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Weights.UpdateWeight
{
    internal class UpdateWeightCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateWeightCommand, bool>
    {
        public async Task<bool> Handle(UpdateWeightCommand request, CancellationToken cancellationToken)
        {
            var weight = await unitOfWork.Repository<Weight>().GetByIdAsync(request.Id, asNoTracking: false)
                ?? throw new NotFoundException(nameof(Weight));

            // Check if another weight with the same value and unit exists (excluding current weight)
            var spec = new GetWeightByValueAndUnitSpec(request.Value, request.Unit);
            var existingWeight = await unitOfWork.Repository<Weight>()
                .GetBySpecificationAsync(spec);

            if (existingWeight != null && existingWeight.Id != request.Id)
            {
                throw new InvalidDataException($"Weight with value '{request.Value}' and unit '{request.Unit}' already exists.");
            }

            weight.Value = request.Value;
            weight.Unit = request.Unit;
            weight.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Repository<Weight>().Update(weight);
            await unitOfWork.CommitAsync();

            return true;
        }
    }
}