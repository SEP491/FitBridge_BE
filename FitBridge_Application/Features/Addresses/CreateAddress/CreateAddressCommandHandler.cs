using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Accounts;
using MediatR;
using AutoMapper;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.Addresses.CreateAddress;

public class CreateAddressCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserUtil userUtil, IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreateAddressCommand, string>
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
        unitOfWork.Repository<Address>().Insert(address);
        await unitOfWork.CommitAsync();
        return address.Id.ToString();
    }
}
