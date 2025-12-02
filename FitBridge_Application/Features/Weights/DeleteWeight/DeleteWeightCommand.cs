using MediatR;

namespace FitBridge_Application.Features.Weights.DeleteWeight
{
    public class DeleteWeightCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
