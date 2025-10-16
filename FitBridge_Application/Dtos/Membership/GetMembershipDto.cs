namespace FitBridge_Application.Dtos.Membership
{
    public class GetMembershipDto
    {
        public Guid Id { get; set; }

        public string ServiceName { get; set; }

        public decimal ServiceCharge { get; set; }

        public int? MaximumHotResearchSlot { get; set; }

        public int? AvailableHotResearchSlot { get; set; }
    }
}
