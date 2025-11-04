using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Subscriptions;
using FitBridge_Application.Specifications;
using FitBridge_Application.Specifications.Subscriptions.GetUserSubscriptionHistory;
using MediatR;

namespace FitBridge_Application.Features.Subscriptions.GetUserSubscriptionHistory;

public class GetUserSubscriptionHistoryQuery : IRequest<PagingResultDto<UserSubscriptionHistoryResponseDto>>
{
    public GetUserSubscriptionHistoryParams Params { get; set; }
}
