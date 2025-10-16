using System;
using FitBridge_Application.Dtos.Bookings;
using MediatR;
using FitBridge_Application.Interfaces.Repositories;
using AutoMapper;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.Bookings.GetTrainingResult;

public class GetTrainingResultQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetTrainingResultQuery, TrainingResultResponseDto>
{
    public async Task<TrainingResultResponseDto> Handle(GetTrainingResultQuery request, CancellationToken cancellationToken)
    {
        var bookingEntity = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.BookingId, false, new List<string>
        {
            "SessionActivities",
            "SessionActivities.ActivitySets",
            "Pt"
        });
        if (bookingEntity == null)
        {
            throw new NotFoundException("Booking not found");
        }
        var trainingResult = _mapper.Map<TrainingResultResponseDto>(bookingEntity);
        return trainingResult;
    }

}
