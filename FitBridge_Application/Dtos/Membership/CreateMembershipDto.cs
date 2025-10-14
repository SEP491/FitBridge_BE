namespace FitBridge_Application.Dtos.Membership
{
    public class CreateMembershipDto
    {
        public Guid Id { get; set; }

        public string ServiceName { get; set; }

        public decimal ServiceCharge { get; set; }

        public int MaximumHotResearchSlot { get; set; }

        public int AvailableHotResearch { get; set; }
    }
}