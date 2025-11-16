using System;
using FitBridge_Application.Dtos.Addresses;
using MediatR;

namespace FitBridge_Application.Features.Addresses.GetAllCustomerAddress;

public class GetAllCustomerAddressesQuery : IRequest<List<AddressResponseDto>>
{
}
