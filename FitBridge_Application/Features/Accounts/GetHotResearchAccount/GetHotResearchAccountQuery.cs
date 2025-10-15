using System;
using FitBridge_Application.Dtos.Accounts.HotResearch;
using FitBridge_Application.Dtos;
using FitBridge_Application.Specifications.Accounts.GetHotResearchAccount;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetHotResearchAccount;

public class GetHotResearchAccountQuery : IRequest<PagingResultDto<HotResearchAccountDto>>
{
    public GetHotResearchAccountParams Params { get; set; }
}
