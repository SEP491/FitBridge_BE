using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Flavours.DeleteFlavour
{
    internal class DeleteFlavourCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteFlavourCommand, bool>
    {
        public async Task<bool> Handle(DeleteFlavourCommand request, CancellationToken cancellationToken)
        {
            var flavour = await unitOfWork.Repository<Flavour>().GetByIdAsync(request.Id, asNoTracking: false)
                ?? throw new NotFoundException(nameof(Flavour));

            unitOfWork.Repository<Flavour>().SoftDelete(flavour);

            await unitOfWork.CommitAsync();

            return true;
        }
    }
}