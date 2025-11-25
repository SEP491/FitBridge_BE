using System;
using FitBridge_Application.Features.Contracts.CreateContract;
using FitBridge_Application.Dtos.Contracts;
using FitBridge_Domain.Entities.Contracts;
using AutoMapper;

namespace FitBridge_Application.MappingProfiles;

public class ContractMappingProfile : Profile
{
    public ContractMappingProfile()
    {
        CreateMap<CreateContractCommand, ContractRecord>();
        CreateMap<ContractRecord, GetContractsDto>();
    }
}
