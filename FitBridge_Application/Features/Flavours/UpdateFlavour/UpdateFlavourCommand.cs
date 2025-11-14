using MediatR;

namespace FitBridge_Application.Features.Flavours.UpdateFlavour
{
    public class UpdateFlavourCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
