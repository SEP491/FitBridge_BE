using MediatR;

namespace FitBridge_Application.Features.Memberships.CreateNewMembership
{
    public class CreateNewMembershipCommand : IRequest
    {
        public string ServiceName { get; set; }

        public decimal ServiceCharge { get; set; }

        public int? MaximumHotResearchSlot { get; set; }

        public int? AvailableHotResearchSlot { get; set; }
    }
}