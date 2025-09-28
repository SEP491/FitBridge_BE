using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.CustomerPurchaseds;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using MediatR;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedFreelancePt;

public class GetCustomerPurchasedFreelancePtQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<GetCustomerPurchasedFreelancePtQuery, PagingResultDto<CustomerPurchasedFreelancePtResponseDto>>
{
    public async Task<PagingResultDto<CustomerPurchasedFreelancePtResponseDto>> Handle(GetCustomerPurchasedFreelancePtQuery request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }
        var customerPurchaseds = await _unitOfWork.Repository<CustomerPurchased>().GetAllWithSpecificationProjectedAsync<CustomerPurchasedFreelancePtResponseDto>(new GetCustomerPurchasedByCustomerIdSpec(userId.Value, request.Params, false), _mapper.ConfigurationProvider);

        var totalItems = await _unitOfWork.Repository<CustomerPurchased>().CountAsync(new GetCustomerPurchasedByCustomerIdSpec(userId.Value, request.Params, false));
        
        return new PagingResultDto<CustomerPurchasedFreelancePtResponseDto>(totalItems, customerPurchaseds);
    }

}
