using System;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Interfaces.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Specifications.GymSlots;
using FitBridge_Application.Interfaces.Services;

namespace FitBridge_Application.Features.GymSlots.CheckMinimumSlot;

public class CheckMinimumSlotCommandHandler(IUnitOfWork _unitOfWork, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IApplicationUserService _applicationUserService) : IRequestHandler<CheckMinimumSlotCommand, CheckMinimumSlotResponseDto>
{
    public async Task<CheckMinimumSlotResponseDto> Handle(CheckMinimumSlotCommand request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }
        var gymPt = await _applicationUserService.GetByIdAsync(userId.Value);
        if (gymPt == null)
        {
            throw new NotFoundException("Gym PT not found");
        }

        var minimumSlot = await _unitOfWork.Repository<PTGymSlot>().GetAllWithSpecificationAsync(new GetMinimumSlotSpecification(request.StartWeek, request.EndWeek, gymPt.Id));
        return new CheckMinimumSlotResponseDto { MinimumSlot = gymPt.MinimumSlot, RegisteredSlot = minimumSlot.Count, IsAccepted = minimumSlot.Count >= gymPt.MinimumSlot };
    }

}
