using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Specifications.GymSlotPts.GetGymPtRegisterSlotForGymOwner;
using MediatR;

namespace FitBridge_Application.Features.GymSlots.GetGymPtRegisterSlotForGymOwner;

public class GetGymPtRegisterSlotForGymOwnerQuery(GetGymPtRegisterSlotForGymOwnerParams parameters) : IRequest<PagingResultDto<GymPtRegisterSlot>>
{
    public GetGymPtRegisterSlotForGymOwnerParams Params { get; set; } = parameters;
}
