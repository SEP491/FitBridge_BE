using MediatR;

namespace FitBridge_Application.Features.Memberships.DeleteMembership
{
    public class DeleteMembershipCommand : IRequest
    {
        public Guid MembershipId { get; set; }
    }
}
