using System;
using MediatR;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Enums.Gyms;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.GymSlotPts.GGetGymSlotPtBySlotIdAndPtId;

namespace FitBridge_Application.Features.GymSlots.RegisterSlot;

public class RegisterSlotCommandHandler(IUnitOfWork _unitOfWork, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IApplicationUserService _applicationUserService) : IRequestHandler<RegisterSlotCommand, bool>
{
    public async Task<bool> Handle(RegisterSlotCommand request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }
        var gymPt = await _applicationUserService.GetByIdAsync(userId.Value);
        var slot = await _unitOfWork.Repository<GymSlot>().GetByIdAsync(request.SlotId);

        if (slot == null)
        {
            throw new NotFoundException("Slot not found");
        }

        if (gymPt.GymOwnerId != slot.GymOwnerId)
        {
            throw new InvalidDataException("Gym PT is not in the same gym as the slot");
        }
        
        var isSlotRegistered = await _unitOfWork.Repository<PTGymSlot>().GetBySpecificationAsync(new GetGymSlotPtBySlotIdAndPtIdSpec(request.SlotId, userId.Value, request.RegisterDate));

        if (isSlotRegistered != null)
        {
            throw new DuplicateException("Slot already registered for this date");
        }
        
        var ptGymSlot = new PTGymSlot
        {
            PTId = userId.Value,
            GymSlotId = request.SlotId,
            Status = PTGymSlotStatus.Activated,
            RegisterDate = request.RegisterDate
        };
        
        _unitOfWork.Repository<PTGymSlot>().Insert(ptGymSlot);
        await _unitOfWork.CommitAsync();
        return true;
    }
}
