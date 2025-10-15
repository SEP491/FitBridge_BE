using System;
using FitBridge_Application.Dtos.SessionActivities;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Trainings;
using AutoMapper;
using MediatR;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Enums.ActivitySets;

namespace FitBridge_Application.Features.SessionActivities;

public class CreateSessionActivityCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<CreateSessionActivityCommand, SessionActivityResponseDto>
{
    public async Task<SessionActivityResponseDto> Handle(CreateSessionActivityCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.BookingId);
        if (booking == null)
        {
            throw new NotFoundException("Booking id not found");
        }
        var mappedEntity = _mapper.Map<CreateSessionActivityCommand, SessionActivity>(request);
        foreach (var activitySet in mappedEntity.ActivitySets)
        {
            if(request.ActivitySetType.Equals(ActivitySetType.Reps))
            {
                activitySet.PlannedPracticeTime = 0; //Because the reps type is not allowed to have planned practice time
            }
            else if(request.ActivitySetType.Equals(ActivitySetType.Time))
            {
                activitySet.PlannedNumOfReps = 0; //Because the time type is not allowed to have planned num of reps
            }
        }
        mappedEntity.BookingId = request.BookingId;
        _unitOfWork.Repository<SessionActivity>().Insert(mappedEntity);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<SessionActivity, SessionActivityResponseDto>(mappedEntity);
    }
}
