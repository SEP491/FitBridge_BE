using System;
using MediatR;

namespace FitBridge_Application.Features.GymSlots.RegisterSlot;

public class RegisterSlotCommand : IRequest<bool>
{
    public Guid SlotId { get; set; }
    public DateOnly RegisterDate { get; set; }
}
