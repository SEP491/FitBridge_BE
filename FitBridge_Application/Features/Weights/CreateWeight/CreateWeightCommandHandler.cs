using FitBridge_Application.Dtos.Weights;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Weights.GetWeightByValueAndUnit;
using FitBridge_Domain.Entities.Ecommerce;
using MediatR;

namespace FitBridge_Application.Features.Weights.CreateWeight
{
    internal class CreateWeightCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<CreateWeightCommand, CreateWeightResponseDto>
    {
        public async Task<CreateWeightResponseDto> Handle(CreateWeightCommand request, CancellationToken cancellationToken)
        {
            var spec = new GetWeightByValueAndUnitSpec(request.Value, request.Unit);
            var weight = await unitOfWork.Repository<Weight>()
                .GetBySpecificationAsync(spec);

            if (weight != null)
            {
                throw new InvalidDataException($"Weight with value '{request.Value}' and unit '{request.Unit}' already exists.");
            }

            var newWeight = new Weight
            {
                Id = Guid.NewGuid(),
                Value = request.Value,
                Unit = request.Unit,
            };

            unitOfWork.Repository<Weight>().Insert(newWeight);

            await unitOfWork.CommitAsync();
            return new CreateWeightResponseDto { Id = newWeight.Id };
        }
    }
}