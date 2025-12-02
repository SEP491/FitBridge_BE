using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Specifications.Messaging.GetAllUsersForMessaging;
using MediatR;

namespace FitBridge_Application.Features.Messaging.GetAllUsersForMessaging;

public class GetAllUsersForMessagingQuery : IRequest<PagingResultDto<MessagingUserDto>>
{
    public GetAllUsersForMessagingParam Params { get; set; }
}
