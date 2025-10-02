using FitBridge_Application.Dtos.FreelancePTPackages;
using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.FreelancePTPackages.GetFreelancePTPackageById
{
    public class GetFreelancePTPackageByIdQuery : IRequest<GetFreelancePTPackageByIdDto>
    {
        [JsonIgnore]
        public Guid PackageId { get; set; }
    }
}