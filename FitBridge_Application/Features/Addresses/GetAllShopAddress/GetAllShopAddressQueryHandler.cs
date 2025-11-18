using System;
using FitBridge_Application.Dtos.Addresses;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Specifications.Addresses.GetAllShopAddress;
using FitBridge_Application.Dtos;

namespace FitBridge_Application.Features.Addresses.GetAllShopAddress;

public class GetAllShopAddressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IApplicationUserService applicationUserService) : IRequestHandler<GetAllShopAddressQuery, PagingResultDto<AddressResponseDto>>
{
    public async Task<PagingResultDto<AddressResponseDto>> Handle(GetAllShopAddressQuery request, CancellationToken cancellationToken)
    {
        var adminAccounts = await applicationUserService.GetUsersByRoleAsync(ProjectConstant.UserRoles.Admin);
        var adminAccountIds = adminAccounts.Select(x => x.Id).ToList();
        var addresses = await unitOfWork.Repository<Address>().GetAllWithSpecificationAsync(new GetAllShopAddressSpec(adminAccountIds, request.Params));
        var totalAddressesCount = await unitOfWork.Repository<Address>().CountAsync(new GetAllShopAddressSpec(adminAccountIds, request.Params));
        return new PagingResultDto<AddressResponseDto>(totalAddressesCount, mapper.Map<List<AddressResponseDto>>(addresses));
    }
}
