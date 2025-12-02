using FitBridge_Application.Dtos.Flavours;
using MediatR;

namespace FitBridge_Application.Features.Flavours.CreateFlavour
{
    public class CreateFlavourCommand : IRequest<CreateFlavourResponseDto>
    {
        public string Name { get; set; }
    }
}