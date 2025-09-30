using System;
using FitBridge_Application.Dtos.GymSlots;
using MediatR;

namespace FitBridge_Application.Features.GymSlots.UpdateGymSlot;

public class UpdateGymSlotCommand : IRequest<SlotResponseDto>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
