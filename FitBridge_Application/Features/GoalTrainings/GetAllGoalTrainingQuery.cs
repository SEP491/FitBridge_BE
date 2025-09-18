using FitBridge_Application.Dtos.GoalTrainings;
using MediatR;

namespace FitBridge_Application.Features.GoalTrainings;

public class GetAllGoalTrainingQuery : IRequest<List<GoalTrainingResponsDto>>
{
}
