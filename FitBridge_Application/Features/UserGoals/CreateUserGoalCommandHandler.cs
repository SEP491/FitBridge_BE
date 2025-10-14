using System;
using FitBridge_Application.Dtos.UserGoals;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using MediatR;
using AutoMapper;

namespace FitBridge_Application.Features.UserGoals;

public class CreateUserGoalCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<CreateUserGoalCommand, UserGoalsDto>
{
    public async Task<UserGoalsDto> Handle(CreateUserGoalCommand request, CancellationToken cancellationToken)
    {
        var customerPurchased = await _unitOfWork.Repository<CustomerPurchased>().GetByIdAsync(request.CustomerPurchasedId, false, new List<string> { "UserGoal" });
        if (customerPurchased == null)
        {
            throw new NotFoundException("Customer purchased not found");
        }
        if(customerPurchased.UserGoal != null)
        {
            throw new BusinessException("User goal already exists");
        }
        var userGoal = _mapper.Map<CreateUserGoalCommand, UserGoal>(request);
        _unitOfWork.Repository<UserGoal>().Insert(userGoal);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<UserGoal, UserGoalsDto>(userGoal);
    }
}
