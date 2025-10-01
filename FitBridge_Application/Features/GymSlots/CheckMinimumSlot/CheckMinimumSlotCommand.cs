using System;
using FitBridge_Application.Dtos.GymSlots;
using MediatR;

namespace FitBridge_Application.Features.GymSlots.CheckMinimumSlot;

public class CheckMinimumSlotCommand : IRequest<CheckMinimumSlotResponseDto>
{
    public DateOnly StartWeek { get; set; }
    public DateOnly EndWeek { get; set; }
}
