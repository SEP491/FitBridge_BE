using FitBridge_Application.Dtos.Flavours;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Flavours.GetAllFlavours;
using FitBridge_Application.Specifications.Flavours.GetFlavourByName;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Flavours.CreateFlavour
{
    internal class CreateFlavourCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<CreateFlavourCommand, CreateFlavourResponseDto>
    {
        public async Task<CreateFlavourResponseDto> Handle(CreateFlavourCommand request, CancellationToken cancellationToken)
        {
            var spec = new GetFlavourByNameSpec(request.Name);
            var flavour = await unitOfWork.Repository<Flavour>()
                .GetBySpecificationAsync(spec);
            if (flavour != null)
            {
                throw new DataValidationFailedException($"Flavour with name '{request.Name}' already exists.");
            }

            var newFlavour = new Flavour
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
            };

            unitOfWork.Repository<Flavour>().Insert(newFlavour);

            await unitOfWork.CommitAsync();
            return new CreateFlavourResponseDto { Id = newFlavour.Id };
        }
    }
}