using System;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Application.Dtos.Addresses;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Exceptions;
using AutoMapper;

namespace FitBridge_Application.Features.Addresses.UpdateAddress;

public class UpdateAddressCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateAddressCommand, AddressResponseDto>
{
    public async Task<AddressResponseDto> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await unitOfWork.Repository<Address>().GetByIdAsync(request.Id);
        if (address == null)
        {
            throw new NotFoundException("Address not found");
        }
        address.ReceiverName = request.ReceiverName ?? address.ReceiverName;
        address.PhoneNumber = request.PhoneNumber ?? address.PhoneNumber;
        address.City = request.City ?? address.City;
        address.District = request.District ?? address.District;
        address.Ward = request.Ward ?? address.Ward;
        address.Street = request.Street ?? address.Street;
        address.HouseNumber = request.HouseNumber ?? address.HouseNumber;
        address.Note = request.Note ?? address.Note;
        address.Latitude = request.Latitude ?? address.Latitude;
        address.Longitude = request.Longitude ?? address.Longitude;
        address.GoogleMapAddressString = request.GoogleMapAddressString ?? address.GoogleMapAddressString;
        address.UpdatedAt = DateTime.UtcNow;
        unitOfWork.Repository<Address>().Update(address);
        await unitOfWork.CommitAsync();
        return mapper.Map<AddressResponseDto>(address);
    }
}
