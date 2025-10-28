using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Bookings;
using FitBridge_Application.Specifications.Bookings.GetGymPtSchedule;
using MediatR;

namespace FitBridge_Application.Features.Bookings.GetGymPtSchedule
{
    public class GetGymPtScheduleQuery : IRequest<PagingResultDto<GetGymPtScheduleResponse>>
    {
        public GetGymPtScheduleParams Params { get; set; } = null!;
    }
}