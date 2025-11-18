using System;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Application.Dtos.Addresses;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Exceptions;
using AutoMapper;
using FitBridge_Application.Specifications.Addresses.GetShopDefaultAddress;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Addresses.GetAllShopAddress;

namespace FitBridge_Application.Features.Addresses.UpdateAddress;

public class UpdateAddressCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IApplicationUserService applicationUserService) : IRequestHandler<UpdateAddressCommand, AddressResponseDto>
{
    public async Task<AddressResponseDto> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var addressEntity = await unitOfWork.Repository<Address>().GetByIdAsync(request.Id);
        if (addressEntity == null)
        {
            throw new NotFoundException("Address not found");
        }
        if (request.IsShopDefaultAddress.HasValue)
        {
            if (request.IsShopDefaultAddress == true)
            {
                await SwitchShopDefaultAddress(addressEntity.Id);
                addressEntity.IsShopDefaultAddress = true;
            }
            if(request.IsShopDefaultAddress == false)
            {
                throw new BusinessException("Shop default address cannot be disabled, please choose another address as shop default address for auto switch");
            }
        }
        addressEntity.ReceiverName = request.ReceiverName ?? addressEntity.ReceiverName;
        addressEntity.PhoneNumber = request.PhoneNumber ?? addressEntity.PhoneNumber;
        addressEntity.City = request.City ?? addressEntity.City;
        addressEntity.District = request.District ?? addressEntity.District;
        addressEntity.Ward = request.Ward ?? addressEntity.Ward;
        addressEntity.Street = request.Street ?? addressEntity.Street;
        addressEntity.HouseNumber = request.HouseNumber ?? addressEntity.HouseNumber;
        addressEntity.Note = request.Note ?? addressEntity.Note;
        addressEntity.Latitude = request.Latitude ?? addressEntity.Latitude;
        addressEntity.Longitude = request.Longitude ?? addressEntity.Longitude;
        addressEntity.GoogleMapAddressString = request.GoogleMapAddressString ?? addressEntity.GoogleMapAddressString;
        addressEntity.UpdatedAt = DateTime.UtcNow;
        unitOfWork.Repository<Address>().Update(addressEntity);
        await unitOfWork.CommitAsync();
        return mapper.Map<AddressResponseDto>(addressEntity);
    }
    
    public async Task SwitchShopDefaultAddress(Guid addressId)
    {
        var adminAccounts = await applicationUserService.GetUsersByRoleAsync(ProjectConstant.UserRoles.Admin);
        var adminAccountIds = adminAccounts.Select(x => x.Id).ToList();
        var addresses = await unitOfWork.Repository<Address>().GetAllWithSpecificationAsync(new GetAllShopAddressSpec(adminAccountIds, exceptAddressId: addressId));
        foreach(var address in addresses)
        {
            address.IsShopDefaultAddress = false;
            unitOfWork.Repository<Address>().Update(address);
        }
    }
}
