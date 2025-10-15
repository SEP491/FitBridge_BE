using FitBridge_Application.Dtos.Membership;
using FitBridge_Application.Specifications.Memberships.GetAllMemberships;
using MediatR;

namespace FitBridge_Application.Features.Memberships.GetAllMemberships
{
    public class GetAllMembershipsQuery : IRequest<(IReadOnlyList<GetMembershipDto> Items, int Total)>
    {
        public GetAllMembershipsParam Params { get; set; }
    }
}
