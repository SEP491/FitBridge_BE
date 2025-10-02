using MediatR;

namespace FitBridge_Application.Features.FreelancePTPackages.DeleteFreelancePTPackage
{
    public class DeleteFreelancePTPackageCommand : IRequest
    {
        public Guid PackageId { get; set; }
    }
}