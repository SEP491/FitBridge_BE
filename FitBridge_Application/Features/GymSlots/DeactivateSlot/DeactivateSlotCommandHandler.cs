using System;
using MediatR;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Application.Specifications.GymSlotPts.GetGymSlotPtByIdAndPtId;

namespace FitBridge_Application.Features.GymSlots.DeactivateSlot;

public class DeactivateSlotCommandHandler(IUnitOfWork _unitOfWork, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<DeactivateSlotCommand, bool>
{
    public async Task<bool> Handle(DeactivateSlotCommand request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);

        var ptGymSlotToDeactivate = await _unitOfWork.Repository<PTGymSlot>().GetBySpecificationAsync(new GetGymSlotPtByIdAndPtId(request.SlotId, userId.Value));
        if(ptGymSlotToDeactivate == null)
        {
            throw new NotFoundException("Pt gym slot not found");
        }
        if(ptGymSlotToDeactivate.Booking != null)
        {
            throw new UpdateFailedException("Slot is booked, cannot deactivate");
        }
        
        _unitOfWork.Repository<PTGymSlot>().Delete(ptGymSlotToDeactivate);
        await _unitOfWork.CommitAsync();
        return true;
    }

}
