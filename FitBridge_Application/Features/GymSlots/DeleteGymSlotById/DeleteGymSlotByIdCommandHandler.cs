using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.GymSlots.DeleteGymSlotById;

public class DeleteGymSlotByIdCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteGymSlotByIdCommand, bool>
{
    public async Task<bool> Handle(DeleteGymSlotByIdCommand request, CancellationToken cancellationToken)
    {
        var entity = await unitOfWork.Repository<GymSlot>().GetByIdAsync(Guid.Parse(request.Id));
        if (entity == null)
        {
            throw new NotFoundException(nameof(GymSlot));
        }
        var result = unitOfWork.Repository<GymSlot>().SoftDelete(entity);
        await unitOfWork.CommitAsync();
        return true;
    }
}
