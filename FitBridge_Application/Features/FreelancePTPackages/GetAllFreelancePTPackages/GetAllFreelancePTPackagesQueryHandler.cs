using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.FreelancePTPackages;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetAvailableCustomerPurchasedByFreelancePackage;
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
        IMapper mapper) : IRequestHandler<GetAllFreelancePTPackagesQuery, AllFreelancePTPackagesDto>
    {
        public async Task<AllFreelancePTPackagesDto> Handle(GetAllFreelancePTPackagesQuery request, CancellationToken cancellationToken)
        {
            var userId = userUtil.GetAccountId(httpContextAccessor.HttpContext) ?? throw new NotFoundException(nameof(ApplicationUser));

            var spec = new GetAllFreelancePTPackagesSpec(request.Params, userId);
            var packages = await unitOfWork.Repository<FreelancePTPackage>()
                .GetAllWithSpecificationAsync(spec);
            var packagesDto = mapper.Map<IReadOnlyList<GetAllFreelancePTPackagesDto>>(packages);
            foreach (var package in packagesDto)
            {
                var countSpec = new GetAvailableCustomerPurchasedByFreelancePackageSpec(package.Id);
                var availableCustomerPurchased = await unitOfWork.Repository<CustomerPurchased>().CountAsync(countSpec);
                package.CurrentUserPurchased = availableCustomerPurchased;
            }
            var summary = new FreelancePtPackageSummaryDto
            {
                TotalPackages = packages.Count,
                TotalPrices = packages.Sum(x => x.Price),
                AveragePrice = Math.Round(packages.Average(x => x.Price), 0),
                AvgSessions = Math.Round((decimal)packages.Average(x => x.NumOfSessions), 0),
                PtMaxCourse = packages.First().Pt.PtMaxCourse,
                PtCurrentCourse = packages.First().Pt.PtCurrentCourse,
            };

            var totalItems = await unitOfWork.Repository<FreelancePTPackage>().CountAsync(spec);

            return new AllFreelancePTPackagesDto
            {
                Packages = new PagingResultDto<GetAllFreelancePTPackagesDto>(totalItems, packagesDto),
                Summary = summary
            };
        }
    }
}