using System;
using MediatR;

namespace FitBridge_Application.Features.Jobs.TriggerInstantJob;

public class InstantTriggerJobCommand : IRequest<bool>
{
    public string JobNamePrefix { get; set; }
    public string JobGroup { get; set; }
    public Guid KeyId { get; set; }
}
