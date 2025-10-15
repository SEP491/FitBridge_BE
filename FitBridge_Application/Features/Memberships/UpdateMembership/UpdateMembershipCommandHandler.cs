using AutoMapper;
using FitBridge_Application.Dtos.Membership;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Memberships.GetMembershipById;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Memberships.UpdateMembership
{
    internal class UpdateMembershipCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<UpdateMembershipCommand, UpdateMembershipDto>
    {
        public async Task<UpdateMembershipDto> Handle(UpdateMembershipCommand request, CancellationToken cancellationToken)
        {
            var spec = new GetMembershipByIdSpecification(request.MembershipId);
            var membership = await unitOfWork.Repository<ServiceInformation>()
                .GetBySpecificationAsync(spec, asNoTracking: false)
                ?? throw new NotFoundException("Membership not found");

            if (!string.IsNullOrWhiteSpace(request.ServiceName))
            {
                membership.ServiceName = request.ServiceName;
            }

            if (request.ServiceCharge.HasValue && request.ServiceCharge >= 0)
            {
                membership.ServiceCharge = request.ServiceCharge.Value;
            }

            if (request.MaximumHotResearchSlot.HasValue && request.MaximumHotResearchSlot.Value >= 0)
            {
                membership.MaximumHotResearchSlot = request.MaximumHotResearchSlot;
            }

            if (request.AvailableHotResearchSlot.HasValue && request.AvailableHotResearchSlot.Value >= 0)
            {
                membership.AvailableHotResearchSlot = request.AvailableHotResearchSlot;
            }

            unitOfWork.Repository<ServiceInformation>().Update(membership);
            await unitOfWork.CommitAsync();

            var dto = mapper.Map<UpdateMembershipDto>(membership);
            return dto;
        }
    }
}