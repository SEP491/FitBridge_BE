using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Specifications.GymSlots;
using MediatR;

namespace FitBridge_Application.Features.GymSlots.GetAllGymSlot;

public class GetGymSlotsQuery : IRequest<PagingResultDto<SlotResponseDto>>
{
    public GetGymSlotParams Params { get; set; }
}
