using System;
using MediatR;

namespace FitBridge_Application.Features.GymSlots.CustomerRegisterSlot;

public class CustomerRegisterSlotCommand : IRequest<bool>
{
    public Guid PtGymSlotId { get; set; }
    public Guid CustomerPurchasedId { get; set; }
}
