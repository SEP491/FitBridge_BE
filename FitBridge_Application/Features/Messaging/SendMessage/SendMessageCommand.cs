using FitBridge_Application.Dtos.Bookings;
using MediatR;

namespace FitBridge_Application.Features.Messaging.SendMessage;

public class SendMessageCommand : IRequest
{
    public Guid ConversationId { get; init; }

    public string? Content { get; init; }

    public Guid? ReplyToMessageId { get; init; }

    public string? ReplyToMessageContent { get; init; }

    public string? ReplyToMessageMediaType { get; init; }

    public string MediaType { get; init; } = string.Empty;

    public Guid? CustomerPurchasedId { get; set; }

    public CreateRequestBookingDto? CreateBookingRequest { get; init; }
}