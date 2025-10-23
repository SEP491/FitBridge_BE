using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Accounts.Search;
using FitBridge_Application.Specifications.Accounts.GetAccountForSearching;
using MediatR;

namespace FitBridge_Application.Features.Accounts.SearchAccounts;
    
public class SearchAccountQuery : IRequest<GetAccountForSearchingResponseDto>
{
    public GetAccountForSearchingParams Params { get; set; }
}
