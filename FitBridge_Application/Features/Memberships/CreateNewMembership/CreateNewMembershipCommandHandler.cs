using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.ServicePackages;
using MediatR;

namespace FitBridge_Application.Features.Memberships.CreateNewMembership
{
    internal class CreateNewMembershipCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<CreateNewMembershipCommand>
    {
        public async Task Handle(CreateNewMembershipCommand request, CancellationToken cancellationToken)
        {
            var newService = new ServiceInformation
            {
                ServiceName = request.ServiceName,
                ServiceCharge = request.ServiceCharge,
                MaximumHotResearchSlot = request.MaximumHotResearchSlot,
                AvailableHotResearchSlot = request.AvailableHotResearchSlot
            };

            unitOfWork.Repository<ServiceInformation>().Insert(newService);

            await unitOfWork.CommitAsync();
        }
    }
}