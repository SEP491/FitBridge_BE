using System;
using FitBridge_Application.Dtos.Bookings;
using FitBridge_Domain.Enums.Trainings;
using MediatR;

namespace FitBridge_Application.Features.Bookings.CreateBooking;

public class CreateRequestBookingCommand : IRequest<List<CreateRequestBookingResponseDto>>
{
    public Guid CustomerPurchasedId { get; set; }

    public List<CreateRequestBookingDto> RequestBookings { get; set; }
}
