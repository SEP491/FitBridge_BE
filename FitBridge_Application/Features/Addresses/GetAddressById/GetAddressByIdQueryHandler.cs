using System;
using FitBridge_Application.Dtos.Addresses;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.Addresses.GetAddressById;

public class GetAddressByIdQueryHandler(IUnitOfWork _unitOfWork, IMapper mapper) : IRequestHandler<GetAddressByIdQuery, AddressResponseDto>
{
    public async Task<AddressResponseDto> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
    {
        var address = await _unitOfWork.Repository<Address>().GetByIdAsync(request.Id);
        if (address == null)
        {
            throw new NotFoundException("Address not found");
        }
        return mapper.Map<AddressResponseDto>(address);
    }
}
