using System;
using FitBridge_Application.Dtos.GymSlots;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace FitBridge_Application.Features.GymSlots.CheckMinimumSlot;

public class CheckMinimumSlotCommand : IRequest<CheckMinimumSlotResponseDto>
{
    [Required]
    public DateOnly StartWeek { get; set; }
    [Required]
    public DateOnly EndWeek { get; set; }
}
