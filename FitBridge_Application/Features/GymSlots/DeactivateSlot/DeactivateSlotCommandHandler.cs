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

        var ptGymSlotToDeactivate = await _unitOfWork.Repository<PTGymSlot>().GetBySpecificationAsync(new GetGymSlotPtByIdAndPtId(request.PtGymSlotId, userId.Value));
        if(ptGymSlotToDeactivate == null)
        {
            throw new NotFoundException($"Pt gym slot with user id {userId.Value} and pt gym slot id {request.PtGymSlotId} not found");
        }
        if(userId.Value != ptGymSlotToDeactivate.PTId)
        {
            throw new UpdateFailedException("You are not authorized to deactivate this pt gym slot");
        }
        if (ptGymSlotToDeactivate.Booking != null)
        {
            throw new UpdateFailedException("Slot is booked, deactivate");
        }
        
        _unitOfWork.Repository<PTGymSlot>().Delete(ptGymSlotToDeactivate);
        await _unitOfWork.CommitAsync();
        return true;
    }

}
