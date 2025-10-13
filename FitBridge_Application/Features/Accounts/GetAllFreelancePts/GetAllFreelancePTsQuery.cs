using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Accounts.FreelancePts;
using FitBridge_Application.Specifications.Accounts.GetAllFreelancePts;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetAllFreelancePts;

public class GetAllFreelancePTsQuery : IRequest<PagingResultDto<GetAllFreelancePTsResponseDto>>
{
    public GetAllFreelancePTsParam Params { get; set; }
}
