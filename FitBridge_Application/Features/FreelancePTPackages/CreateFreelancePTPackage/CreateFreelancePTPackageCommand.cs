using FitBridge_Application.Dtos.FreelancePTPackages;
using MediatR;

namespace FitBridge_Application.Features.FreelancePTPackages.CreateFreelancePTPackage
{
    public class CreateFreelancePTPackageCommand : IRequest<CreateFreelancePTPackageDto>
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int DurationInDays { get; set; }

        public int SessionDurationInMinutes { get; set; }

        public int NumOfSessions { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public Guid PtId { get; set; }
    }
}