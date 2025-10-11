using System;
using AutoMapper;
using FitBridge_Application.Dtos.SessionActivities;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.SessionActivities.UpdateSessionActivity;

public class UpdateSessionActivityCommandHandler(IUnitOfWork unitOfWork, IMapper _mapper) : IRequestHandler<UpdateSessionActivityCommand, SessionActivitiyResponseDto>
{

    public async Task<SessionActivitiyResponseDto> Handle(UpdateSessionActivityCommand request, CancellationToken cancellationToken)
    {
        var sessionActivity = await unitOfWork.Repository<SessionActivity>().GetByIdAsync(request.SessionActivityId);
        if (sessionActivity == null)
        {
            throw new NotFoundException(nameof(SessionActivity));
        }
        sessionActivity.ActivityType = request.ActivityType;
        sessionActivity.ActivityName = request.ActivityName;
        sessionActivity.MuscleGroups = request.MuscleGroups;

        unitOfWork.Repository<SessionActivity>().Update(sessionActivity);
        await unitOfWork.CommitAsync();
        return _mapper.Map<SessionActivitiyResponseDto>(sessionActivity);
    }
}
