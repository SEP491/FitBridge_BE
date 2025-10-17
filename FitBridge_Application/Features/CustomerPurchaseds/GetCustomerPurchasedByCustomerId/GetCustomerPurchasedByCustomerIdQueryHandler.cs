using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Accounts;
using FitBridge_Application.Dtos.CustomerPurchaseds;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;
using FitBridge_Domain.Entities.Gyms;
using MediatR;

namespace FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedByCustomerId
{
    internal class GetCustomerPurchasedByCustomerIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetCustomerPurchasedByCustomerIdQuery, PagingResultDto<CustomerPurchasedFreelancePtResponseDto>>
    {
        public async Task<PagingResultDto<CustomerPurchasedFreelancePtResponseDto>> Handle(GetCustomerPurchasedByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new GetCustomerPurchasedByCustomerIdSpec(request.CustomerId, request.Params, isGymCourse: false);

            var customerPurchased = await unitOfWork.Repository<CustomerPurchased>()
                .GetAllWithSpecificationProjectedAsync<CustomerPurchasedFreelancePtResponseDto>(spec, mapper.ConfigurationProvider);

            var totalCount = await unitOfWork.Repository<CustomerPurchased>()
                .CountAsync(spec);

            return new PagingResultDto<CustomerPurchasedFreelancePtResponseDto>(totalCount, customerPurchased);
        }
    }
}