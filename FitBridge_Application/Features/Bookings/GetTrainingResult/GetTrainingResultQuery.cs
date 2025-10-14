using System;
using FitBridge_Application.Dtos.Bookings;
using MediatR;

namespace FitBridge_Application.Features.Bookings.GetTrainingResult;

public class GetTrainingResultQuery : IRequest<TrainingResultResponseDto>
{
    public Guid BookingId { get; set; }
}
