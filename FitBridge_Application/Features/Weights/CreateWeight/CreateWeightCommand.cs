using FitBridge_Application.Dtos.Weights;
using MediatR;

namespace FitBridge_Application.Features.Weights.CreateWeight
{
    public class CreateWeightCommand : IRequest<CreateWeightResponseDto>
    {
        public double Value { get; set; }
        public string Unit { get; set; }
    }
}
