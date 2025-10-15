using System;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.ActivitySets.DeleteActivity;

public class DeleteActivitySetByIdCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteActivitySetByIdCommand, bool>
{
    public async Task<bool> Handle(DeleteActivitySetByIdCommand request, CancellationToken cancellationToken)
    {
        var activitySet = await unitOfWork.Repository<ActivitySet>().GetByIdAsync(request.Id);
        if (activitySet == null)
        {
            throw new NotFoundException("Activity set not found");
        }
        unitOfWork.Repository<ActivitySet>().Delete(activitySet);
        await unitOfWork.CommitAsync();
        return true;
    }
}
