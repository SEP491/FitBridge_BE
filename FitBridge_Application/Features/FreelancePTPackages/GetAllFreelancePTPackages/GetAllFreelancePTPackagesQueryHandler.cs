using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.FreelancePTPackages;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.FreelancePtPackages.GetAllFreelancePTPackages;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.FreelancePTPackages.GetAllFreelancePTPackages
{
    internal class GetAllFreelancePTPackagesQueryHandler(
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IUserUtil userUtil,
        IMapper mapper) : IRequestHandler<GetAllFreelancePTPackagesQuery, PagingResultDto<GetAllFreelancePTPackagesDto>>
    {
        public async Task<PagingResultDto<GetAllFreelancePTPackagesDto>> Handle(GetAllFreelancePTPackagesQuery request, CancellationToken cancellationToken)
        {
            var userId = userUtil.GetAccountId(httpContextAccessor.HttpContext) ?? throw new NotFoundException(nameof(ApplicationUser));

            var spec = new GetAllFreelancePTPackagesSpec(request.Params, userId);
            var packages = await unitOfWork.Repository<FreelancePTPackage>()
                .GetAllWithSpecificationProjectedAsync<GetAllFreelancePTPackagesDto>(spec, mapper.ConfigurationProvider);

            var totalItems = await unitOfWork.Repository<FreelancePTPackage>().CountAsync(spec);

            return new PagingResultDto<GetAllFreelancePTPackagesDto>(totalItems, packages);
        }
    }
}