using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Bookings;
using FitBridge_Application.Specifications.Bookings.GetCustomerBookings;
using MediatR;

namespace FitBridge_Application.Features.Bookings.GetCustomerBooking;

public class GetCustomerBookingsQuery : IRequest<PagingResultDto<GetCustomerBookingsResponse>>
{
    public GetCustomerBookingsParams Params { get; set; }
}
