using System;
using FitBridge_Application.Dtos.Flavours;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Application.Specifications.Flavours.GetAllFlavours;

namespace FitBridge_Application.Features.Flavours.GetAllFlavours;

public class GetAllFlavoursQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetAllFlavoursQuery, List<FlavourResponseDto>>  
{
    public async Task<List<FlavourResponseDto>> Handle(GetAllFlavoursQuery request, CancellationToken cancellationToken)
    {
        var flavours = await _unitOfWork.Repository<Flavour>().GetAllWithSpecificationProjectedAsync<FlavourResponseDto>(new GetAllFlavoursSpecification(), _mapper.ConfigurationProvider);
        return flavours.ToList();
    }
}
