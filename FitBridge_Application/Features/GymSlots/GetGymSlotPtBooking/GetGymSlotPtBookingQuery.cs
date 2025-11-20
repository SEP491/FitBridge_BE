using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Specifications.GymSlotPts.GetGymSlotPtBooking;
using MediatR;

namespace FitBridge_Application.Features.GymSlots.GetGymSlotPtBooking;

public class GetGymSlotPtBookingQuery(GetGymSlotPtBookingParams parameters) : IRequest<PagingResultDto<GymSlotPtBookingDto>>
{
    public GetGymSlotPtBookingParams Params { get; set; } = parameters;
}
