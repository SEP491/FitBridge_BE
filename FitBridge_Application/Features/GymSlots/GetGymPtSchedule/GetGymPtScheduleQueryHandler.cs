using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.GymSlots;
using AutoMapper;
using MediatR;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Specifications.GymSlotPts.GetPtGymSlotForPtSchedules;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.GymSlots.GetGymPtSchedule;

public class GetGymPtScheduleQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IApplicationUserService _applicationUserService) : IRequestHandler<GetGymPtScheduleQuery, PagingResultDto<PTSlotScheduleResponse>>
{
    public async Task<PagingResultDto<PTSlotScheduleResponse>> Handle(GetGymPtScheduleQuery request, CancellationToken cancellationToken)
    {
        var ptEntity = await _applicationUserService.GetByIdAsync(request.Params.PtId);
        if (ptEntity == null)
        {
            throw new NotFoundException("PT not found");
        }
        var ptGymSlots = await _unitOfWork.Repository<PTGymSlot>().GetAllWithSpecificationProjectedAsync<PTSlotScheduleResponse>(new GetPtGymSlotForPtScheduleSpec(request.Params.PtId, request.Params), _mapper.ConfigurationProvider);

        var totalItems = await _unitOfWork.Repository<PTGymSlot>().CountAsync(new GetPtGymSlotForPtScheduleSpec(request.Params.PtId, request.Params));
        return new PagingResultDto<PTSlotScheduleResponse>(totalItems, ptGymSlots);
    }

}
