using FitBridge_Application.Dtos.Membership;
using MediatR;

namespace FitBridge_Application.Features.Memberships.UpdateMembership
{
    public class UpdateMembershipCommand : IRequest<UpdateMembershipDto>
    {
        public Guid MembershipId { get; set; }

        public string? ServiceName { get; set; }

        public decimal? ServiceCharge { get; set; }

        public int? MaximumHotResearchSlot { get; set; }

        public int? AvailableHotResearchSlot { get; set; }
    }
}