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
using FitBridge_Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Interfaces.Utils;
namespace FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchased;

public class GetCustomerPurchasedQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<GetCustomerPurchasedQuery, PagingResultDto<CustomerPurchasedResponseDto>>
{
    public async Task<PagingResultDto<CustomerPurchasedResponseDto>> Handle(GetCustomerPurchasedQuery request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }
        var customerPurchaseds = await _unitOfWork.Repository<CustomerPurchased>().GetAllWithSpecificationProjectedAsync<CustomerPurchasedResponseDto>(new GetCustomerPurchasedByCustomerIdSpec(userId.Value, request.Params), _mapper.ConfigurationProvider);

        var totalItems = await _unitOfWork.Repository<CustomerPurchased>().CountAsync(new GetCustomerPurchasedByCustomerIdSpec(userId.Value, request.Params));
        
        return new PagingResultDto<CustomerPurchasedResponseDto>(totalItems, customerPurchaseds);
    }
}
