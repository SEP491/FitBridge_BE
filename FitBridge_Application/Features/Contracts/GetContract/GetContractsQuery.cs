using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Contracts;
using FitBridge_Application.Specifications.Contracts.GetContract;
using MediatR;

namespace FitBridge_Application.Features.Contracts.GetContract;

public class GetContractsQuery : IRequest<PagingResultDto<GetContractsDto>>
{
    public GetContractsParams Params { get; set; }
}
