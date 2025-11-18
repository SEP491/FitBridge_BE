using System;
using FitBridge_Application.Dtos.Addresses;
using FitBridge_Application.Specifications.Addresses.GetAllShopAddress;
using MediatR;
using FitBridge_Application.Dtos;
namespace FitBridge_Application.Features.Addresses.GetAllShopAddress;

public class GetAllShopAddressQuery(GetAllShopAddressParams parameters) : IRequest<PagingResultDto<AddressResponseDto>>
{
    public GetAllShopAddressParams Params { get; set; } = parameters;
}
