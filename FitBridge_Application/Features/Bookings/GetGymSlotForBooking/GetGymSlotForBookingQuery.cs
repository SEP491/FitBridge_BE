using System;
using FitBridge_Application.Dtos.Bookings;
using MediatR;
using FitBridge_Application.Specifications.Bookings.GetGymSlotForBooking;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Dtos;

namespace FitBridge_Application.Features.Bookings.GetGymSlotForBooking;

public class GetGymSlotForBookingQuery : IRequest<PagingResultDto<GetPtGymSlotForBookingResponse>>
{
    public GetGymSlotForBookingParams Params { get; set; }
}
