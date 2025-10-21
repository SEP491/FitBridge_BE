using System;
using FitBridge_Application.Dtos.ActivitySets;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Domain.Entities.Trainings;
using AutoMapper;
using FitBridge_Domain.Exceptions;
namespace FitBridge_Application.Features.ActivitySets.UpdateActivityProgress;

public class UpdateActivityProgressCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<UpdateActivityProgressCommand, ActivitySetResponseDto>
{
    public async Task<ActivitySetResponseDto> Handle(UpdateActivityProgressCommand request, CancellationToken cancellationToken)
    {
        var activitySet = await _unitOfWork.Repository<ActivitySet>().GetByIdAsync(request.ActivitySet.ActivitySetId);
        if (activitySet == null)
        {
            throw new NotFoundException(nameof(ActivitySet));
        }
        activitySet.IsCompleted = request.ActivitySet.IsCompleted;
        activitySet.WeightLifted = request.ActivitySet.WeightLifted ?? activitySet.WeightLifted;
        activitySet.NumOfReps = request.ActivitySet.NumOfReps ?? activitySet.NumOfReps;
        activitySet.PracticeTime = request.ActivitySet.PracticeTime ?? activitySet.PracticeTime;
        activitySet.RestTime = request.ActivitySet.RestTime ?? activitySet.RestTime;
        activitySet.ActualDistance = request.ActivitySet.ActualDistance ?? activitySet.ActualDistance;
        _unitOfWork.Repository<ActivitySet>().Update(activitySet);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<ActivitySetResponseDto>(activitySet);
    }
}
