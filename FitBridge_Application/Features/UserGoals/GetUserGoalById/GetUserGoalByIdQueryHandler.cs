using System;
using FitBridge_Application.Dtos.UserGoals;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.UserGoals.GetUserGoalById;

public class GetUserGoalByIdQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetUserGoalByIdQuery, UserGoalsDto>
{
    public async Task<UserGoalsDto> Handle(GetUserGoalByIdQuery request, CancellationToken cancellationToken)
    {
        var userGoal = await _unitOfWork.Repository<UserGoal>().GetByIdAsync(request.Id);
        if (userGoal == null)
        {
            throw new NotFoundException("User goal not found");
        }
        return _mapper.Map<UserGoal, UserGoalsDto>(userGoal);
    }

}
