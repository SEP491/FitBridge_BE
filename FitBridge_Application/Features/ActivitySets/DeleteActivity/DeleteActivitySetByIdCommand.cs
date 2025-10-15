using System;
using MediatR;

namespace FitBridge_Application.Features.ActivitySets.DeleteActivity;

public class DeleteActivitySetByIdCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
