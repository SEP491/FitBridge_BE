using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.FreelancePTPackages.UpdateFreelancePTPackage
{
    public class UpdateFreelancePTPackageCommand : IRequest
    {
        [JsonIgnore]
        public Guid PackageId { get; set; }

        public string? Name { get; set; }

        public decimal? Price { get; set; }

        public int? DurationInDays { get; set; }

        public int? SessionDurationInMinutes { get; set; }

        public int? NumOfSessions { get; set; }

        public string? Description { get; set; }
        public bool? IsDisplayed { get; set; }

        public string? ImageUrl { get; set; }
    }
}