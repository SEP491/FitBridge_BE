using MediatR;

namespace FitBridge_Application.Features.Refund.RefundItem
{
    public class RefundItemCommand : IRequest
    {
        public Guid OrderItemId { get; set; }
    }
}