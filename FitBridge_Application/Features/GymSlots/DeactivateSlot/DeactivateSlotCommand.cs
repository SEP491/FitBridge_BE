using System;
using MediatR;

namespace FitBridge_Application.Features.GymSlots.DeactivateSlot;

public class DeactivateSlotCommand : IRequest<bool>
{
    public Guid PtGymSlotId { get; set; }
}
