using System;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Dtos.GymSlots;
using AutoMapper;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Specifications.GymSlots;
using FitBridge_Application.Services;

namespace FitBridge_Application.Features.GymSlots.UpdateGymSlot;

public class UpdateGymSlotCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, SystemConfigurationService systemConfigurationService) : IRequestHandler<UpdateGymSlotCommand, SlotResponseDto>
{
    public async Task<SlotResponseDto> Handle(UpdateGymSlotCommand request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.Repository<GymSlot>().GetByIdAsync(request.Id);
        if (entity == null)
        {
            throw new NotFoundException(nameof(GymSlot));
        }
        await ValidateGymSlot(request, entity);
        entity.Name = request.Name ?? entity.Name;
        if (request.StartTime != default)
        {
            entity.StartTime = request.StartTime;
        }
        if (request.EndTime != default)
        {
            entity.EndTime = request.EndTime;
        }
        _unitOfWork.Repository<GymSlot>().Update(entity);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<GymSlot, SlotResponseDto>(entity);
    }

    public async Task ValidateGymSlot(UpdateGymSlotCommand request, GymSlot entity)
    {
        if (request.StartTime >= request.EndTime)
        {
            throw new DataValidationFailedException("Start time must be less than end time");
        }

        var defaultGymSlotDuration = (int)await systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.GymSlotDuration);
        if (request.EndTime - request.StartTime < TimeSpan.FromHours(defaultGymSlotDuration))
        {
            throw new DataValidationFailedException("Gym slot duration must be more than " + defaultGymSlotDuration + " hour");
        }
        
        if(request.Name != entity.Name)
        {
            var result = await _unitOfWork.Repository<GymSlot>().GetBySpecificationAsync(new GetGymSlotByNameSpec(request.Name));
            if (result != null)
            {
                throw new DuplicateException("Gym slot name already exists");
            }
        }
        if (request.StartTime != entity.StartTime || request.EndTime != entity.EndTime)
        {
            var gymSlot = await _unitOfWork.Repository<GymSlot>().GetAllWithSpecificationAsync(new GetGymSlotByTimeRangeSpec(request.StartTime, request.EndTime));
            if (gymSlot.Count > 0)
            {
                foreach(var slot in gymSlot)
                {
                    if(slot.Id != entity.Id)
                    {
                        throw new DuplicateException("Gym slot overlapping with existing gym slot");
                    }
                }
            }
        }
    }

}
