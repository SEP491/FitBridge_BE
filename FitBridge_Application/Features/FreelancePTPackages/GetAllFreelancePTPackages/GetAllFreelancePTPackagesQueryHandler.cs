using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.FreelancePTPackages;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.FreelancePtPackages.GetAllFreelancePTPackages;
using FitBridge_Domain.Entities.Gyms;
using MediatR;

namespace FitBridge_Application.Features.FreelancePTPackages.GetAllFreelancePTPackages
{
    internal class GetAllFreelancePTPackagesQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetAllFreelancePTPackagesQuery, PagingResultDto<GetAllFreelancePTPackagesDto>>
    {
        public async Task<PagingResultDto<GetAllFreelancePTPackagesDto>> Handle(GetAllFreelancePTPackagesQuery request, CancellationToken cancellationToken)
        {
            var spec = new GetAllFreelancePTPackagesSpec(request.Params);
            var packages = await unitOfWork.Repository<FreelancePTPackage>()
                .GetAllWithSpecificationProjectedAsync<GetAllFreelancePTPackagesDto>(spec, mapper.ConfigurationProvider);

            var totalItems = await unitOfWork.Repository<FreelancePTPackage>().CountAsync(spec);

            return new PagingResultDto<GetAllFreelancePTPackagesDto>(totalItems, packages);
        }
    }
}