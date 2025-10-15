using System;
using AutoMapper;
using FitBridge_Application.Dtos.SessionActivities;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.SessionActivities.UpdateSessionActivity;

public class UpdateSessionActivityCommandHandler(IUnitOfWork unitOfWork, IMapper _mapper) : IRequestHandler<UpdateSessionActivityCommand, SessionActivityResponseDto>
{

    public async Task<SessionActivityResponseDto> Handle(UpdateSessionActivityCommand request, CancellationToken cancellationToken)
    {
        var sessionActivity = await unitOfWork.Repository<SessionActivity>().GetByIdAsync(request.SessionActivityId, false, new List<string> { "Booking", "ActivitySets" });
        if (sessionActivity == null)
        {
            throw new NotFoundException(nameof(SessionActivity));
        }
        if (sessionActivity.ActivitySets.Count > 0
        && !sessionActivity.ActivitySetType.Equals(request.ActivitySetType))
        {
            throw new BusinessException("Cannot change activity set type after activity sets are created");
        }
        sessionActivity.ActivityType = request.ActivityType;
        sessionActivity.ActivityName = request.ActivityName;
        sessionActivity.MuscleGroups = request.MuscleGroups;
        sessionActivity.ActivitySetType = request.ActivitySetType;
        await unitOfWork.CommitAsync();
        return _mapper.Map<SessionActivityResponseDto>(sessionActivity);
    }
}
