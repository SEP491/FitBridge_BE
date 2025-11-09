using AutoMapper;
using FitBridge_Application.Dtos.Membership;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Memberships.GetAllMemberships;
using FitBridge_Domain.Entities.ServicePackages;
using MediatR;

namespace FitBridge_Application.Features.Memberships.GetAllMemberships
{
    internal class GetAllMembershipsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetAllMembershipsQuery, (IReadOnlyList<GetMembershipDto> Items, int Total)>
    {
        public async Task<(IReadOnlyList<GetMembershipDto> Items, int Total)> Handle(GetAllMembershipsQuery request, CancellationToken cancellationToken)
        {
            // var spec = new GetAllMembershipsSpecification(request.Params);
            // var memberships = await unitOfWork.Repository<ServiceInformation>()
            //     .GetAllWithSpecificationAsync(spec);

            // var totalCount = await unitOfWork.Repository<ServiceInformation>()
            //     .CountAsync(spec);

            // var dtos = mapper.Map<IReadOnlyList<GetMembershipDto>>(memberships);
            // return (dtos, totalCount);
            return (new List<GetMembershipDto>(), 0);
        }
    }
}



