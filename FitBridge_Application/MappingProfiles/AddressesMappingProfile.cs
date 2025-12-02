using System;
using AutoMapper;
using FitBridge_Application.Dtos.Addresses;
using FitBridge_Application.Features.Addresses.CreateAddress;
using FitBridge_Domain.Entities.Accounts;

namespace FitBridge_Application.MappingProfiles;

public class AddressesMappingProfile : Profile
{
    public AddressesMappingProfile()
    {
        CreateMap<CreateAddressCommand, Address>();
        CreateMap<Address, AddressResponseDto>();
    }
}
