using MediatR;

namespace FitBridge_Application.Features.Weights.UpdateWeight
{
    public class UpdateWeightCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
    }
}
