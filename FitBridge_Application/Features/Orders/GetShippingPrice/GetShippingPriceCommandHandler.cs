using System;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Orders;
using FitBridge_Application.Dtos.Shippings;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Orders.GetShippingPrice;

public class GetShippingPriceCommandHandler(IUnitOfWork _unitOfWork, IAhamoveService _ahamoveService) : IRequestHandler<GetShippingPriceCommand, ShippingEstimateDto>
{
    public async Task<ShippingEstimateDto> Handle(GetShippingPriceCommand request, CancellationToken cancellationToken)
    {
        var toAddress = await _unitOfWork.Repository<Address>().GetByIdAsync(request.AddressId);
        if (toAddress == null)
        {
            throw new NotFoundException("Address not found");
        }
        var fromAddress = await _unitOfWork.Repository<Address>().GetByIdAsync(Guid.Parse(ProjectConstant.DefaultShopAddressId));
        if (fromAddress == null)
        {
            throw new NotFoundException("From address not found");
        }
        var pickUpaddress = new AhamovePathDto
        {
            Lat = fromAddress.Latitude,
            Lng = fromAddress.Longitude,
            Address = fromAddress.GoogleMapAddressString,
            ShortAddress = fromAddress.Ward,
            Name = "Shop",
            Mobile = fromAddress.PhoneNumber
        };
        var dropOffAddress = new AhamovePathDto
        {
            Lat = toAddress.Latitude,
            Lng = toAddress.Longitude,
            Address = toAddress.GoogleMapAddressString,
            ShortAddress = toAddress.Ward,
            Name = "Customer",
            Mobile = toAddress.PhoneNumber
        };
        var path = new List<AhamovePathDto>
        {
            pickUpaddress,
            dropOffAddress
        };
        var requestDto = new AhamovePriceEstimateDto
        {
            OrderTime = 0,
            Path = path,
            Services = new List<AhamoveServiceDto>
            {
                new AhamoveServiceDto
                {
                    Id = ProjectConstant.DefaultAhamoveServiceId
                }
            },
            PaymentMethod = "CASH"
        };
        var shippingPriceDto = await _ahamoveService.GetShippingPriceAsync(requestDto);
        return shippingPriceDto;
    }
}
