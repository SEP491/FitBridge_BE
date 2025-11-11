using System;
using FitBridge_Application.Dtos.Weights;
using MediatR;

namespace FitBridge_Application.Features.Weights.GetAllWeights;

public class GetAllWeightsQuery : IRequest<List<WeightResponseDto>>
{
}
