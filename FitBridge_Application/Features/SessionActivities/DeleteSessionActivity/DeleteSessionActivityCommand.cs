using MediatR;

namespace FitBridge_Application.Features.SessionActivities.DeleteSessionActivity;

public class DeleteSessionActivityCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}