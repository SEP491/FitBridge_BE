using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Specifications.GymSlotPts.GetGymPtRegisterSlotForGymOwner;
using FitBridge_Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Features.GymSlots.GetGymPtRegisterSlotForGymOwner;

public class GetGymPtRegisterSlotForGymOwnerQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetGymPtRegisterSlotForGymOwnerQuery, PagingResultDto<GymPtRegisterSlot>>
{
    public async Task<PagingResultDto<GymPtRegisterSlot>> Handle(GetGymPtRegisterSlotForGymOwnerQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetGymPtRegisterSlotForGymOwnerSpec(request.Params);
        var ptGymSlots = await _unitOfWork.Repository<PTGymSlot>().GetAllWithSpecificationAsync(spec);
        var ptGymSlotDtos = _mapper.Map<List<GymPtRegisterSlot>>(ptGymSlots);
        var totalItems = await _unitOfWork.Repository<PTGymSlot>().CountAsync(spec);
        return new PagingResultDto<GymPtRegisterSlot>(totalItems, ptGymSlotDtos);
    }

}
