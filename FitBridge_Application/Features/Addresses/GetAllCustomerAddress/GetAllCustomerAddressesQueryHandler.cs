using System;
using AutoMapper;
using FitBridge_Application.Dtos.Addresses;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Addresses.GetAllCustomerAddress;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Addresses.GetAllCustomerAddress;

public class GetAllCustomerAddressesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserUtil userUtil, IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetAllCustomerAddressesQuery, List<AddressResponseDto>>
{
    public async Task<List<AddressResponseDto>> Handle(GetAllCustomerAddressesQuery request, CancellationToken cancellationToken)
    {
        var customerId = userUtil.GetAccountId(httpContextAccessor.HttpContext);
        if (customerId == null)
        {
            throw new NotFoundException("Customer not found");
        }
        var addresses = await unitOfWork.Repository<Address>().GetAllWithSpecificationAsync(new GetAllCustomerAddressesSpec(customerId.Value));
        return mapper.Map<List<AddressResponseDto>>(addresses);
    }
}
