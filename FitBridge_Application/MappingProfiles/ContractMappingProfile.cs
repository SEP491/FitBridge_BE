using System;
using FitBridge_Application.Features.Contracts.CreateContract;
using FitBridge_Application.Dtos.Contracts;
using FitBridge_Domain.Entities.Contracts;
using AutoMapper;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.MappingProfiles;

public class ContractMappingProfile : Profile
{
    public ContractMappingProfile()
    {
        CreateMap<CreateContractCommand, ContractRecord>();
        CreateMap<ContractRecord, GetContractsDto>();
        CreateMap<ApplicationUser, NonContractUserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));
    }
}
