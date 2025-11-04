using AutoMapper;
using FitBridge_Application.Dtos.Membership;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Memberships.GetMembershipById;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Memberships.GetMembershipById
{
    internal class GetMembershipByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetMembershipByIdQuery, GetMembershipDto>
    {
        public async Task<GetMembershipDto> Handle(GetMembershipByIdQuery request, CancellationToken cancellationToken)
        {
            // var spec = new GetMembershipByIdSpecification(request.MembershipId);
            // var membership = await unitOfWork.Repository<ServiceInformation>()
            //     .GetBySpecificationAsync(spec);

            // if (membership == null)
            // {
            //     throw new NotFoundException("Membership not found");
            // }

            // var dto = mapper.Map<GetMembershipDto>(membership);
            // return dto;
            return new GetMembershipDto();
        }
    }
}