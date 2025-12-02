using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Flavours.GetFlavourByName;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Flavours.UpdateFlavour
{
    internal class UpdateFlavourCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateFlavourCommand, bool>
    {
        public async Task<bool> Handle(UpdateFlavourCommand request, CancellationToken cancellationToken)
        {
            var flavour = await unitOfWork.Repository<Flavour>().GetByIdAsync(request.Id, asNoTracking: false)
                ?? throw new NotFoundException(nameof(Flavour));

            // Check if another flavour with the same name exists (excluding current flavour)
            var spec = new GetFlavourByNameSpec(request.Name);
            var existingFlavour = await unitOfWork.Repository<Flavour>()
                .GetBySpecificationAsync(spec);

            if (existingFlavour != null && existingFlavour.Id != request.Id)
            {
                throw new DataValidationFailedException($"Flavour with name '{request.Name}' already exists.");
            }

            flavour.Name = request.Name;
            flavour.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Repository<Flavour>().Update(flavour);
            await unitOfWork.CommitAsync();

            return true;
        }
    }
}