using FitBridge_Application.Dtos.GymAssets;

namespace FitBridge_Application.Dtos.Gym
{
    public class GetAllGymsDto
    {
        public Guid Id { get; set; }

        public string GymName { get; set; } = string.Empty;

        public string RepresentName { get; set; } = string.Empty;

        public string GymAddress { get; set; } = string.Empty;

        public List<GymImageDto> GymImages { get; set; } = [];

        public double Longitude { get; set; } = 0.0;

        public double Latitude { get; set; } = 0.0;

        public bool HotResearch { get; set; } = false;

        public string GymDescription { get; set; } = string.Empty;
        public DateOnly? GymFoundationDate { get; set; }
        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }
        public DateTime CreatedAt { get; set; }

        // Business Information
        public string? TaxCode { get; set; }
        public string? AvatarUrl { get; set; }
        public string? CitizenIdNumber { get; set; }
        public string? IdentityCardPlace { get; set; }
        public string? CitizenCardPermanentAddress { get; set; }
        public DateOnly? IdentityCardDate { get; set; }
        public string? FrontCitizenIdUrl { get; set; }
        public string? BackCitizenIdUrl { get; set; }
        public List<GymAssetResponseDto> GymAssets { get; set; } = [];
    }
}