using System;
using AutoMapper;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Specifications.GymSlots;
using FitBridge_Application.Commons.Constants;

namespace FitBridge_Application.Features.GymSlots.CreateGymSlot;

public class CreateGymSlotCommandHandler(IMapper _mapper, IUnitOfWork _unitOfWork, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<CreateGymSlotCommand, CreateNewSlotResponse>
{
    public async Task<CreateNewSlotResponse> Handle(CreateGymSlotCommand request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("Gym owner not found");
        }
        await ValidateGymSlot(request.Request);
        var mappedEntity = _mapper.Map<CreateNewSlotResponse, GymSlot>(request.Request);
        mappedEntity.GymOwnerId = userId.Value;
        _unitOfWork.Repository<GymSlot>().Insert(mappedEntity);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<GymSlot, CreateNewSlotResponse>(mappedEntity);
    }

    public async Task ValidateGymSlot(CreateNewSlotResponse request)
    {
        if (request.StartTime >= request.EndTime)
        {
            throw new DataValidationFailedException("Start time must be less than end time");
        }

        if (request.EndTime - request.StartTime < TimeSpan.FromHours(ProjectConstant.GymSlotDuration))
        {
            throw new DataValidationFailedException("Gym slot duration must be more than " + ProjectConstant.GymSlotDuration + " hour");
        }
        
        var gymSlot = await _unitOfWork.Repository<GymSlot>().GetBySpecificationAsync(new GetGymSlotForCreateValidationSpecification(request));
        if (gymSlot != null)
        {
            throw new DuplicateException("Gym slot name already exists or overlapping with existing gym slot");
        }
    }
}
