using System;
using AutoMapper;
using FitBridge_Application.Dtos.ActivitySets;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.ActivitySets.UpdateActivitySet;

public class UpdateActivitySetCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateActivitySetCommand, ActivitySetResponseDto>
{
    public async Task<ActivitySetResponseDto> Handle(UpdateActivitySetCommand request, CancellationToken cancellationToken)
    {
        var activitySet = await unitOfWork.Repository<ActivitySet>().GetByIdAsync(request.ActivitySetId);
        if (activitySet == null)
        {
            throw new NotFoundException("Activity set not found Id: " + request.ActivitySetId);
        }
        if(activitySet.IsCompleted)
        {
            throw new BusinessException("Activity set already completed, cannot update");
        }
        activitySet.WeightLifted = request.WeightLifted ?? activitySet.WeightLifted;
        activitySet.NumOfReps = request.NumOfReps ?? activitySet.NumOfReps;
        activitySet.UpdatedAt = DateTime.UtcNow;
        unitOfWork.Repository<ActivitySet>().Update(activitySet);
        await unitOfWork.CommitAsync();
        return mapper.Map<ActivitySetResponseDto>(activitySet);
    }

}
