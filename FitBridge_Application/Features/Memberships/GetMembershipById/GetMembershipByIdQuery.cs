using FitBridge_Application.Dtos.Membership;
using MediatR;

namespace FitBridge_Application.Features.Memberships.GetMembershipById
{
    public class GetMembershipByIdQuery : IRequest<GetMembershipDto>
    {
        public Guid MembershipId { get; set; }
    }
}