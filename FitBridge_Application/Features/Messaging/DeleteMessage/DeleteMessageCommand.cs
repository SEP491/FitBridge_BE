using MediatR;

namespace FitBridge_Application.Features.Messaging.DeleteMessage
{
    public class DeleteMessageCommand : IRequest
    {
        public Guid MessageId { get; set; }
    }
}