using System;
using MediatR;

namespace FitBridge_Application.Features.GymSlots.DeleteGymSlotById;

public class DeleteGymSlotByIdCommand(string id) : IRequest<bool>
{
    public string Id { get; set; } = id;
}
