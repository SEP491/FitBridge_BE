using System;
using FitBridge_Application.Dtos.Weights;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Application.Specifications.Weights.GetAllWeights;

namespace FitBridge_Application.Features.Weights.GetAllWeights;

public class GetAllWeightsQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetAllWeightsQuery, List<WeightResponseDto>>
{
    public async Task<List<WeightResponseDto>> Handle(GetAllWeightsQuery request, CancellationToken cancellationToken)
    {
        var weights = await _unitOfWork.Repository<Weight>().GetAllWithSpecificationProjectedAsync<WeightResponseDto>(new GetAllWeightsSpecification(), _mapper.ConfigurationProvider);
        return weights.ToList();
    }

}
