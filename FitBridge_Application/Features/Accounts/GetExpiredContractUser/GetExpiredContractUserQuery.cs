using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Contracts;
using FitBridge_Application.Specifications.Accounts.GetExpiredContractUser;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetExpiredContractUser;

public class GetExpiredContractUserQuery(GetExpiredContractUserParams parameters) : IRequest<PagingResultDto<NonContractUserDto>>
{
    public GetExpiredContractUserParams Params { get; set; } = parameters;

}
