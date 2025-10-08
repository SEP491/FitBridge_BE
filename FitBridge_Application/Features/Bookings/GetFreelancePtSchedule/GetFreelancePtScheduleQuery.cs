using System;
using FitBridge_Application.Dtos.Bookings;
using MediatR;
using FitBridge_Application.Specifications.Bookings.GetFreelancePtSchedule;
using FitBridge_Application.Dtos;

namespace FitBridge_Application.Features.Bookings.GetFreelancePtSchedule;

public class GetFreelancePtScheduleQuery : IRequest<PagingResultDto<GetFreelancePtScheduleResponse>>
{
    public GetFreelancePtScheduleParams Params { get; set; }
}
