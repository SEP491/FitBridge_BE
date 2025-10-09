using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByFreelancePtId;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using MediatR;

namespace FitBridge_Application.Features.CustomerPurchaseds.CheckCustomerPurchased;

public class CheckCustomerPurchasedCommandHandler(IUnitOfWork _unitOfWork, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<CheckCustomerPurchasedCommand, Guid>
{
    public async Task<Guid> Handle(CheckCustomerPurchasedCommand request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }
        var customerPurchased = await _unitOfWork.Repository<CustomerPurchased>().GetBySpecificationAsync(new GetCustomerPurchasedByFreelancePtIdSpec(request.PtId, userId.Value));
        if (customerPurchased == null)
        {
            throw new NotFoundException("Customer purchased not found");
        }
        return customerPurchased.Id;
    }
}
