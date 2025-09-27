using System;
using FitBridge_Application.Dtos.GymSlots;
using MediatR;

namespace FitBridge_Application.Features.GymSlots.CreateGymSlot;

public class CreateGymSlotCommand : IRequest<CreateNewSlotResponse>
{
    public CreateNewSlotResponse Request { get; set; }
}
