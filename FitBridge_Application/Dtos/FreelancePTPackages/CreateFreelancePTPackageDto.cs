namespace FitBridge_Application.Dtos.FreelancePTPackages
{
    public class CreateFreelancePTPackageDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int DurationInDays { get; set; }

        public int SessionDurationInMinutes { get; set; }

        public int NumOfSessions { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
    }
}