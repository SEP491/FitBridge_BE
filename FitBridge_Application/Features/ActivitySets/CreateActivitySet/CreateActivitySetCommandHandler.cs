using System;
using AutoMapper;
using FitBridge_Application.Dtos.ActivitySets;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.ActivitySets.CreateActivitySet;

public class CreateActivitySetCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateActivitySetCommand, ActivitySetResponseDto>
{
    public async Task<ActivitySetResponseDto> Handle(CreateActivitySetCommand request, CancellationToken cancellationToken)
    {
        var sessionActivity = await unitOfWork.Repository<SessionActivity>().GetByIdAsync(request.SessionActivityId);
        if (sessionActivity == null)
        {
            throw new NotFoundException("Session activity not found Id: " + request.SessionActivityId);
        }
        var activitySet = new ActivitySet
        {
            SessionActivityId = request.SessionActivityId,
            WeightLifted = request.WeightLifted ?? 0,
            PlannedNumOfReps = request.NumOfReps ?? 0,
            PlannedPracticeTime = request.PlannedPracticeTime ?? 0.0
        };
        unitOfWork.Repository<ActivitySet>().Insert(activitySet);
        await unitOfWork.CommitAsync();
        return mapper.Map<ActivitySetResponseDto>(activitySet);
    }
}
