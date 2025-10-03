using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.FreelancePtPackages.GetFreelancePtPackageById;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.FreelancePTPackages.DeleteFreelancePTPackage
{
    internal class DeleteFreelancePTPackageCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteFreelancePTPackageCommand>
    {
        public async Task Handle(DeleteFreelancePTPackageCommand request, CancellationToken cancellationToken)
        {
            var spec = new GetFreelancePtPackageByIdSpec(request.PackageId);
            var existingPackage = await unitOfWork.Repository<FreelancePTPackage>()
                .GetBySpecificationAsync(spec, asNoTracking: false)
                ?? throw new NotFoundException(nameof(FreelancePTPackage));

            unitOfWork.Repository<FreelancePTPackage>().SoftDelete(existingPackage);

            await unitOfWork.CommitAsync();
        }
    }
}