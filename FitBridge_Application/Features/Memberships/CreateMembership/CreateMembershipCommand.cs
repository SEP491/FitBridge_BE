using FitBridge_Application.Dtos.Membership;
using MediatR;

namespace FitBridge_Application.Features.Memberships.CreateMembership
{
    public class CreateMembershipCommand : IRequest<CreateMembershipDto>
    {
        public string ServiceName { get; set; }

        public decimal ServiceCharge { get; set; }

        public int? MaximumHotResearchSlot { get; set; }

        public int? AvailableHotResearchSlot { get; set; }
    }
}