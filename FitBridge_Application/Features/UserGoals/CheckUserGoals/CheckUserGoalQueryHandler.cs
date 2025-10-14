using System;
using MediatR;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Features.UserGoals.CheckUserGoals;

public class CheckUserGoalQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<CheckUserGoalQuery, bool>
{
    public async Task<bool> Handle(CheckUserGoalQuery request, CancellationToken cancellationToken)
    {
        var customerPurchased = await _unitOfWork.Repository<CustomerPurchased>().GetByIdAsync(request.CustomerPurchasedId, false, new List<string> { "UserGoal" });
        if (customerPurchased == null)
        {
            throw new NotFoundException("Customer purchased not found");
        }
        if(customerPurchased.UserGoal == null)
        {
            return false;
        }
        return true;
    }
}
