using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Specifications.Messaging.GetConversations;
using MediatR;

namespace FitBridge_Application.Features.Messaging.GetConversations
{
    public class GetConversationsQuery : IRequest<PagingResultDto<GetConversationsDto>>
    {
        public GetConversationsParam Params { get; set; }
    }
}