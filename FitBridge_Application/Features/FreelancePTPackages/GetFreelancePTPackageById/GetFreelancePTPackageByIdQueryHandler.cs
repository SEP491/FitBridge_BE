using AutoMapper;
using FitBridge_Application.Dtos.FreelancePTPackages;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.FreelancePtPackages.GetFreelancePtPackageById;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.FreelancePTPackages.GetFreelancePTPackageById
{
    internal class GetFreelancePTPackageByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetFreelancePTPackageByIdQuery, GetFreelancePTPackageByIdDto>
    {
        public async Task<GetFreelancePTPackageByIdDto> Handle(GetFreelancePTPackageByIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new GetFreelancePtPackageByIdSpec(request.PackageId);
            var existingPackage = await unitOfWork.Repository<FreelancePTPackage>()
                .GetBySpecificationProjectedAsync<GetFreelancePTPackageByIdDto>(spec, mapper.ConfigurationProvider)
                ?? throw new NotFoundException(nameof(FreelancePTPackage));

            return existingPackage;
        }
    }
}