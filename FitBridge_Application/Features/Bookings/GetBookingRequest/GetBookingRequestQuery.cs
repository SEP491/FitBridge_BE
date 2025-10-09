using System;
using MediatR;
using FitBridge_Application.Specifications.Bookings.GetBookingRequests;
using FitBridge_Application.Dtos.Bookings;
using FitBridge_Application.Dtos;

namespace FitBridge_Application.Features.Bookings.GetBookingRequest;

public class GetBookingRequestQuery : IRequest<PagingResultDto<GetBookingRequestResponse>>
{
    public GetBookingRequestParams Params { get; set; }
}
