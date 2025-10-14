using System;
using MediatR;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.UserGoals.CheckUserGoals;

public class CheckUserGoalQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<CheckUserGoalQuery, bool>
{
    public async Task<bool> Handle(CheckUserGoalQuery request, CancellationToken cancellationToken)
    {
        var userGoal = await _unitOfWork.Repository<UserGoal>().GetByIdAsync(request.CustomerPurchasedId);
        if (userGoal == null)
        {
            throw new NotFoundException("User goal of customer purchased id: " + request.CustomerPurchasedId);
        }
        return true;
    }
}
