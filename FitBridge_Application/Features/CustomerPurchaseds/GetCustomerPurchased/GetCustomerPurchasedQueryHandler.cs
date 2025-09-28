using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.CustomerPurchaseds;
using MediatR;
using FitBridge_Application.Interfaces;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;
using AutoMapper;

namespace FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchased;

public class GetCustomerPurchasedQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetCustomerPurchasedQuery, PagingResultDto<CustomerPurchasedResponseDto>>
{
    public async Task<PagingResultDto<CustomerPurchasedResponseDto>> Handle(GetCustomerPurchasedQuery request, CancellationToken cancellationToken)
    {
        var customerPurchaseds = await _unitOfWork.Repository<CustomerPurchased>().GetAllWithSpecificationProjectedAsync<CustomerPurchasedResponseDto>(new GetCustomerPurchasedByCustomerIdSpec(request.Params.CustomerId), _mapper.ConfigurationProvider);

        var totalItems = await _unitOfWork.Repository<CustomerPurchased>().CountAsync(new GetCustomerPurchasedByCustomerIdSpec(request.Params.CustomerId));
        
        return new PagingResultDto<CustomerPurchasedResponseDto>(totalItems, customerPurchaseds);
    }
}
