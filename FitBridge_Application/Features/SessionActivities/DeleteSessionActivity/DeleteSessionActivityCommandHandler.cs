using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.SessionActivities.DeleteSessionActivity;

public class DeleteSessionActivityCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<DeleteSessionActivityCommand, bool>
{
    public async Task<bool> Handle(DeleteSessionActivityCommand request, CancellationToken cancellationToken)
    {
        var sessionActivity = await _unitOfWork.Repository<SessionActivity>().GetByIdAsync(request.Id, false, new List<string> { "ActivitySets" });
        if (sessionActivity == null)
        {
            throw new NotFoundException("Session activity not found");
        }
        _unitOfWork.Repository<SessionActivity>().Delete(sessionActivity);
        await _unitOfWork.CommitAsync();
        return true;
    }
}
