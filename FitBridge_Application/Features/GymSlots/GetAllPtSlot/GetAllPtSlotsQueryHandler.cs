using System;
using MediatR;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Specifications.GymSlots;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Specifications.GymSlotPts.GetGymSlotPtByIdAndPtId;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Interfaces.Services;
using AutoMapper;
using FitBridge_Application.Dtos;

namespace FitBridge_Application.Features.GymSlots.GetAllPtSlot;

public class GetAllPtSlotsQueryHandler(IUnitOfWork _unitOfWork, IApplicationUserService _applicationUserService, IMapper _mapper) : IRequestHandler<GetAllPtSlotsQuery, PagingResultDto<GetPTSlot>>
{
    public async Task<PagingResultDto<GetPTSlot>> Handle(GetAllPtSlotsQuery request, CancellationToken cancellationToken)
    {
        var ptEntity = await _applicationUserService.GetByIdAsync(request.Params.PtId);
        var ptGymSlots = await _unitOfWork.Repository<GymSlot>().GetAllWithSpecificationAsync(new GetGymSlotForPtRegisterSpec(ptEntity.GymOwnerId.Value, request.Params));
        
        foreach (var ptGymSlot in ptGymSlots)
        {
            ptGymSlot.PTGymSlots = ptGymSlot.PTGymSlots.Where(x => x.PTId == request.Params.PtId && x.RegisterDate == request.Params.RegisterDate).ToList();
        }
        var ptGymSlotsDto = _mapper.Map<List<GetPTSlot>>(ptGymSlots);

        var totalItems = await _unitOfWork.Repository<GymSlot>().CountAsync(new GetGymSlotForPtRegisterSpec(ptEntity.GymOwnerId.Value, request.Params));
        return new PagingResultDto<GetPTSlot>(totalItems, ptGymSlotsDto);
    }
}
