using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Accounts;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Accounts.GetFreelancePtCustomers;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetFreelancePtCustomerPurchased;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Accounts.GetFreelancePtCustomers
{
    internal class GetFreelancePtCustomerQueryHandler(
        IApplicationUserService applicationUserService,
        IMapper mapper,
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetFreelancePtCustomerQuery, PagingResultDto<GetCustomersDto>>
    {
        public async Task<PagingResultDto<GetCustomersDto>> Handle(GetFreelancePtCustomerQuery request, CancellationToken cancellationToken)
        {
            var ptId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException(nameof(ApplicationUser));
            var spec = new GetFreelancePtCustomersSpec(ptId, request.Params);

            var customerDtos = await applicationUserService.GetAllUserWithSpecProjectedAsync<GetCustomersDto>(
                spec, mapper.ConfigurationProvider);

            var totalCount = await applicationUserService
                .CountAsync(spec);

            return new PagingResultDto<GetCustomersDto>(totalCount, customerDtos);
        }
    }
}