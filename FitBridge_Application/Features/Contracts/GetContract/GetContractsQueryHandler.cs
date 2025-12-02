using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Contracts;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Contracts.GetContract;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Contracts;

namespace FitBridge_Application.Features.Contracts.GetContract;

public class GetContractsQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetContractsQuery, PagingResultDto<GetContractsDto>>
{
    public async Task<PagingResultDto<GetContractsDto>> Handle(GetContractsQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetContractsSpec(request.Params);
        var contracts = await _unitOfWork.Repository<ContractRecord>().GetAllWithSpecificationAsync(spec);
        var totalItems = await _unitOfWork.Repository<ContractRecord>().CountAsync(spec);
        var dtos = _mapper.Map<IReadOnlyList<GetContractsDto>>(contracts);
        return new PagingResultDto<GetContractsDto>(totalItems, dtos);
    }
}
