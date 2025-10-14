using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Accounts;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetFreelancePtCustomerPurchased;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Accounts.GetFreelancePtCustomers
{
    internal class GetFreelancePtCustomerQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetFreelancePtCustomerQuery, PagingResultDto<GetCustomersDto>>
    {
        public async Task<PagingResultDto<GetCustomersDto>> Handle(GetFreelancePtCustomerQuery request, CancellationToken cancellationToken)
        {
            var ptId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException(nameof(ApplicationUser));
            var spec = new GetFreelancePtCustomerPurchasedSpec(ptId, request.Params);

            var customerPurchased = await unitOfWork.Repository<CustomerPurchased>()
                    .GetAllWithSpecificationAsync(spec);

            var customers = customerPurchased
                .GroupBy(cp => cp.CustomerId)
                .Select(g => g.First().Customer)
                .Select(customer => mapper.Map<GetCustomersDto>(customer))
                .ToList();

            var totalCount = await unitOfWork.Repository<CustomerPurchased>()
                .CountAsync(new GetFreelancePtCustomerPurchasedSpec(ptId, request.Params));

            return new PagingResultDto<GetCustomersDto>(totalCount, customers);
        }
    }
}