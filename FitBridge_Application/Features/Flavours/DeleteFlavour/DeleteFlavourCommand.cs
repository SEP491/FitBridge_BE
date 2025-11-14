using MediatR;

namespace FitBridge_Application.Features.Flavours.DeleteFlavour
{
    public class DeleteFlavourCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
