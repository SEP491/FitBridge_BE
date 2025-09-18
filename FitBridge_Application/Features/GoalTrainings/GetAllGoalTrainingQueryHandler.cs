using FitBridge_Application.Dtos.GoalTrainings;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.GoalTrainings;
using FitBridge_Domain.Entities.Trainings;
using System.Linq;
using MediatR;
using FitBridge_Domain.Entities.Accounts;
using AutoMapper;

namespace FitBridge_Application.Features.GoalTrainings;

public class GetAllGoalTrainingQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetAllGoalTrainingQuery, List<GoalTrainingResponsDto>>
{
    public async Task<List<GoalTrainingResponsDto>> Handle(GetAllGoalTrainingQuery request, CancellationToken cancellationToken)
    {
        var goalTrainings = await _unitOfWork.Repository<GoalTraining>().GetAllWithSpecificationAsync(new GetAllGoalTrainingSpecification());
        var goalTrainingsDto = _mapper.Map<List<GoalTrainingResponsDto>>(goalTrainings);
        return goalTrainingsDto;
    }
}
