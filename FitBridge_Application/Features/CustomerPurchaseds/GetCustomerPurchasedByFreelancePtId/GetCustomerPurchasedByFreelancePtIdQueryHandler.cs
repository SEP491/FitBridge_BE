using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByFreelancePtId;
using FitBridge_Domain.Entities.Gyms;
using MediatR;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.FreelancePTPackages;
using AutoMapper;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedForFreelancePt;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedByFreelancePtId;

public class GetCustomerPurchasedByFreelancePtIdQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<GetCustomerPurchasedByFreelancePtIdQuery, PagingResultDto<GetCustomerPurchasedForFreelancePt>>
{
    public async Task<PagingResultDto<GetCustomerPurchasedForFreelancePt>> Handle(GetCustomerPurchasedByFreelancePtIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }
        var customerPurchaseds = await _unitOfWork.Repository<CustomerPurchased>().GetAllWithSpecificationProjectedAsync<GetCustomerPurchasedForFreelancePt>
        (new GetCustomerPurchasedForFreelancePtSpec(userId.Value, request.Params), _mapper.ConfigurationProvider);

        var totalItems = await _unitOfWork.Repository<CustomerPurchased>().CountAsync(new GetCustomerPurchasedForFreelancePtSpec(userId.Value, request.Params));

        return new PagingResultDto<GetCustomerPurchasedForFreelancePt>(totalItems, customerPurchaseds);
    }
}
