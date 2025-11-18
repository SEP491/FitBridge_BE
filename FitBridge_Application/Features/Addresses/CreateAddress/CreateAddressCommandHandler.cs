using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Accounts;
using MediatR;
using AutoMapper;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Specifications.Addresses.GetAllShopAddress;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Interfaces.Services;

namespace FitBridge_Application.Features.Addresses.CreateAddress;

public class CreateAddressCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserUtil userUtil, IHttpContextAccessor httpContextAccessor, IApplicationUserService applicationUserService) : IRequestHandler<CreateAddressCommand, string>
{
    public async Task<string> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        var customerId = userUtil.GetAccountId(httpContextAccessor.HttpContext);
        if (customerId == null)
        {
            throw new NotFoundException("Customer not found");
        }

        var address = mapper.Map<CreateAddressCommand, Address>(request);
        address.CustomerId = customerId.Value;
        if (request.IsShopDefaultAddress.HasValue && request.IsShopDefaultAddress == true)
        {
            await SwitchShopDefaultAddress();
            address.IsShopDefaultAddress = true;
        } else {
            address.IsShopDefaultAddress = false;
        }
        unitOfWork.Repository<Address>().Insert(address);
        await unitOfWork.CommitAsync();
        return address.Id.ToString();
    }
    
    public async Task SwitchShopDefaultAddress()
    {
        var adminAccounts = await applicationUserService.GetUsersByRoleAsync(ProjectConstant.UserRoles.Admin);
        var adminAccountIds = adminAccounts.Select(x => x.Id).ToList();
        var addresses = await unitOfWork.Repository<Address>().GetAllWithSpecificationAsync(new GetAllShopAddressSpec(adminAccountIds));
        foreach(var address in addresses)
        {
            address.IsShopDefaultAddress = false;
            unitOfWork.Repository<Address>().Update(address);
        }
    }
}
