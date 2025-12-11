namespace FitBridge_Application.Dtos.FreelancePTPackages
{
    public class GetAllFreelancePTPackagesDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int DurationInDays { get; set; }

        public int SessionDurationInMinutes { get; set; }

        public int NumOfSessions { get; set; }
        public int TotalPurchased { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
        public bool? IsPurchased { get; set; }
        public bool IsDisplayed { get; set; }
        public int CurrentUserPurchased { get; set; }
        public Guid PtId { get; set; }
        public bool IsEnabled { get; set; }
    }
}