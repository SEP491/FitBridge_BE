using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Bookings;
using FitBridge_Application.Specifications.Bookings.GetBookingHistory;
using MediatR;

namespace FitBridge_Application.Features.Bookings.GetBookingHistory
{
    public class GetBookingHistoryQuery : IRequest<PagingResultDto<GetBookingHistoryResponseDto>>
    {
  public GetBookingHistoryParams Params { get; set; } = null!;
    }
}
