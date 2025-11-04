using AutoMapper;
using FitBridge_Application.Dtos.Membership;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.ServicePackages;
using MediatR;

namespace FitBridge_Application.Features.Memberships.CreateMembership
{
    internal class CreateMembershipCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<CreateMembershipCommand, CreateMembershipDto>
    {
        public async Task<CreateMembershipDto> Handle(CreateMembershipCommand request, CancellationToken cancellationToken)
        {
            // var newService = new ServiceInformation
            // {
            //     ServiceName = request.ServiceName,
            //     ServiceCharge = request.ServiceCharge,
            //     MaximumHotResearchSlot = request.MaximumHotResearchSlot,
            //     AvailableHotResearchSlot = request.AvailableHotResearchSlot
            // };

            // unitOfWork.Repository<ServiceInformation>().Insert(newService);

            // await unitOfWork.CommitAsync();

            // var dto = mapper.Map<CreateMembershipDto>(newService);
            // return dto;
            return new CreateMembershipDto();
        }
    }
}