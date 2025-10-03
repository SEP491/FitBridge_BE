using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Specifications.GymSlots;
using FitBridge_Domain.Entities.Gyms;
using MediatR;

namespace FitBridge_Application.Features.GymSlots.GetGymPtSchedule;

public class GetGymPtScheduleQuery : IRequest<PagingResultDto<PTSlotScheduleResponse>>
{
    public GetGymPtScheduleParams Params { get; set; }
}
